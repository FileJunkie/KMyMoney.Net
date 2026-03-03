using System.Reflection;
using Dropbox.Api;
using KMyMoney.Net.Core;
using KMyMoney.Net.Models;
using KMyMoney.Net.TelegramBot.Controllers;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Services;
using KMyMoney.Net.TelegramBot.Settings;
using KMyMoney.Net.TelegramBot.Telegram;
using KMyMoney.Net.Tests.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUser = Telegram.Bot.Types.User;

namespace KMyMoney.Net.TelegramBot.IntegrationTests;

internal sealed class KMyMoneyIntegrationTestContext
{
    private const long DefaultUserId = 123;
    private const long DefaultChatId = 456;
    private readonly ServiceProvider _serviceProvider;
    private readonly ITelegramBotClient _botClient;
    private readonly TestSettingsPersistenceLayer _persistenceLayer;
    private readonly TestDropboxFileAccessor _fileAccessor;

    public KMyMoneyIntegrationTestContext(KMyMoneyIntegrationTestContextOptions? options = null)
    {
        options ??= new();

        _botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(_botClient);
        _persistenceLayer = new TestSettingsPersistenceLayer();
        _fileAccessor = new TestDropboxFileAccessor();
        var fileAccessService = new TestDropboxFileAccessService(
            _persistenceLayer,
            _fileAccessor,
            options.ThrowUnexpectedFileAccessError);
        var oAuth2Helper = new TestDropboxOAuth2HelperWrapper(options.OAuth2Responses);

        SeedVirtualKmyFile("/wallet.kmy");

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"Telegram:{nameof(TelegramSettings.ApiToken)}"] = "test-telegram-token",
                [$"Dropbox:{nameof(DropboxSettings.ApiKey)}"] = "test-dropbox-key",
                [$"Dropbox:{nameof(DropboxSettings.ApiSecret)}"] = "test-dropbox-secret",
                [$"Dropbox:{nameof(DropboxSettings.RedirectUri)}"] = "https://bot.test/dropbox/callback",
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<ISettingsPersistenceLayer>(_persistenceLayer);
        services.AddSingleton<ITelegramBotClientWrapper>(botWrapper);
        services.ConfigureServices(configuration).AddControllers();

        services.RemoveAll<IFileAccessService>();
        services.AddSingleton(fileAccessService);
        services.AddSingleton<IFileAccessService>(sp => sp.GetRequiredService<TestDropboxFileAccessService>());

        services.RemoveAll<IDropboxOAuth2HelperWrapper>();
        services.AddSingleton(oAuth2Helper);
        services.AddSingleton<IDropboxOAuth2HelperWrapper>(sp => sp.GetRequiredService<TestDropboxOAuth2HelperWrapper>());

        _serviceProvider = services.BuildServiceProvider();
    }

    public async Task OnMessageAsync(
        string? text,
        long userId = DefaultUserId,
        long chatId = DefaultChatId,
        ChatType chatType = ChatType.Private,
        UpdateType updateType = UpdateType.Message,
        CancellationToken cancellationToken = default)
    {
        var updateHandler = _serviceProvider.GetRequiredService<IUpdateHandler>();
        await updateHandler.OnMessageAsync(
            BuildMessage(text, userId, chatId, chatType),
            updateType,
            cancellationToken);
    }

    public Task<IActionResult> OnDropboxCallbackAsync(string code, string state, CancellationToken cancellationToken = default)
    {
        var controller = new DropboxController(
            _serviceProvider.GetRequiredService<ISettingsPersistenceLayer>(),
            _serviceProvider.GetRequiredService<IOptions<DropboxSettings>>(),
            _serviceProvider.GetRequiredService<IDropboxOAuth2HelperWrapper>(),
            _serviceProvider.GetRequiredService<IUpdateHandler>(),
            _serviceProvider.GetRequiredService<ILogger<DropboxController>>());
        return controller.CallbackAsync(code, state, cancellationToken);
    }

    public async Task<KMyMoneyFile> LoadTestFileAsync(string path = "/wallet.kmy")
    {
        var fileUri = new Uri($"{_fileAccessor.UriPrefix}{NormalizePath(path)}");
        return await KMyMoneyLoader.LoadFileAsync(_fileAccessor, fileUri);
    }

    public Task SetUserSettingAsync(
        long userId,
        UserSettings setting,
        string? value,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default) =>
        _persistenceLayer.SetUserSettingByUserIdAsync(userId, setting, value, expiresIn, cancellationToken);

    public Task<string?> GetUserSettingAsync(
        long userId,
        UserSettings setting,
        CancellationToken cancellationToken = default) =>
        _persistenceLayer.GetUserSettingByUserIdAsync(userId, setting, cancellationToken);

    public Task<string?> GetSavedValueAsync(
        string key,
        CancellationToken cancellationToken = default) =>
        _persistenceLayer.GetSavedValueByKeyAsync(key, cancellationToken);

    public void AdvanceTimeBy(TimeSpan duration) => _persistenceLayer.AdvanceBy(duration);

    public IReadOnlyList<SendMessageRequest> SentMessages() => _botClient
        .ReceivedCalls()
        .Where(c => c.GetMethodInfo().Name == nameof(ITelegramBotClient.SendRequest))
        .Select(c => c.GetArguments().FirstOrDefault())
        .OfType<SendMessageRequest>()
        .ToArray();

    public void ClearMessages() => _botClient.ClearReceivedCalls();

    public static Message BuildMessage(
        string? text,
        long userId = DefaultUserId,
        long chatId = DefaultChatId,
        ChatType chatType = ChatType.Private) =>
        new()
        {
            Text = text,
            From = new TelegramUser { Id = userId, Username = "test-user" },
            Chat = new Chat { Id = chatId, Type = chatType }
        };

    public static OAuth2Response CreateOAuth2Response(
        string accessToken,
        int expiresInSeconds = 300)
    {
        var constructor = typeof(OAuth2Response)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .First();

        return (OAuth2Response)constructor.Invoke(
        [
            accessToken,
            "refresh-token",
            "uid",
            "state",
            "bearer",
            expiresInSeconds,
            new[] { "account_id" }
        ]);
    }

    private void SeedVirtualKmyFile(string path)
    {
        var baseRoot = TestUtils.CreateTestKmyMoneyFileRoot();
        baseRoot.Accounts.Values =
        [
            new()
            {
                Id = "A000001",
                Name = "Checking Account",
                Type = "Asset",
                Currency = "USD"
            },
            new()
            {
                Id = "A000002",
                Name = "Cash",
                Type = "Asset",
                Currency = "USD"
            },
            new()
            {
                Id = "A000003",
                Name = "Closed account",
                Type = "Asset",
                Currency = "USD",
                KeyValuePairs = new KeyValuePairs
                {
                    Pair =
                    [
                        new KMyMoney.Net.Models.KeyValuePair
                        {
                            Key = "mm-closed",
                            Value = "yes"
                        }
                    ]
                }
            }
        ];
        baseRoot.Prices.Values =
        [
            new()
            {
                From = "USD",
                To = "EUR",
                Price =
                [
                    new PriceObj
                    {
                        Source = "test",
                        Date = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd"),
                        Price = "1/1"
                    }
                ]
            }
        ];

        _fileAccessor.SeedVirtualFile(path, TestUtils.CreateCompressedStream(baseRoot));
    }

    private static string NormalizePath(string path) => path.StartsWith('/') ? path : $"/{path}";
}
