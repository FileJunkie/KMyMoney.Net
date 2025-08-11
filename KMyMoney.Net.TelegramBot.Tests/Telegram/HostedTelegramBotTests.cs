using KMyMoney.Net.TelegramBot.Commands;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Services;
using KMyMoney.Net.TelegramBot.Settings;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Telegram.Bot;
using Telegram.Bot.Requests;

namespace KMyMoney.Net.TelegramBot.Tests.Telegram;

public class HostedTelegramBotTests
{
    [Fact]
    public async Task StartAsync_ShouldSetWebhook_WhenWebhookUriIsProvided()
    {
        // Arrange
        var webhookUri = new Uri("https://example.com/webhook");
        var settings = new TelegramSettings { ApiToken = "dummy_token", WebhookUri = webhookUri };
        var context = new TestContext(settings);

        // Act
        await context.HostedTelegramBot.StartAsync(CancellationToken.None);

        // Assert
        await context.SettingsPersistenceLayer.Received(1).SetSavedValueByKeyAsync(
            "telegramSecret",
            Arg.Any<string>(),
            cancellationToken: CancellationToken.None);

        await context.BotClient.Received(1)
            .SendRequest(
                Arg.Is<SetWebhookRequest>(r =>
                    r.Url == webhookUri.ToString()),
                CancellationToken.None);
    }

    [Fact]
    public async Task StartAsync_ShouldDeleteWebhook_WhenWebhookUriIsNotProvided()
    {
        // Arrange
        var settings = new TelegramSettings { ApiToken = "dummy_token", WebhookUri = null };
        var context = new TestContext(settings);

        // Act
        var action = async () => await context.HostedTelegramBot.StartAsync(CancellationToken.None);

        // Assert
        action.ShouldThrow<Exception>()
            .Message.ShouldStartWith("What kind of type");
        await context.BotClient.Received(1).SendRequest(Arg.Any<DeleteWebhookRequest>(), CancellationToken.None);
    }

    [Fact]
    public async Task StartAsync_ShouldSetBotCommands()
    {
        // Arrange
        var command = Substitute.For<ICommand>();
        command.Command.Returns("test");
        command.Description.Returns("A test command");
        var commands = new[] { command };
        var settings = new TelegramSettings { ApiToken = "dummy_token", WebhookUri = new Uri("https://example.com") };
        var context = new TestContext(settings, commands);

        // Act
        await context.HostedTelegramBot.StartAsync(CancellationToken.None);

        // Assert
        await context.BotClient.Received(1)
            .SendRequest(
                Arg.Is<SetMyCommandsRequest>(r => r.Commands.Single().Command == "test"),
                CancellationToken.None);
    }

    [Fact]
    public async Task StopAsync_ShouldCallBotWrapperStopAsync()
    {
        // Arrange
        var settings = new TelegramSettings { ApiToken = "dummy_token" };
        var context = new TestContext(settings);

        // Act
        await context.HostedTelegramBot.StopAsync(CancellationToken.None);

        // Assert
        await context.BotWrapper.Received(1).StopAsync();
    }

    private class TestContext
    {
        public HostedTelegramBot HostedTelegramBot { get; }
        public ITelegramBotClientWrapper BotWrapper { get; }
        public ITelegramBotClient BotClient { get; }
        public ISettingsPersistenceLayer SettingsPersistenceLayer { get; }

        public TestContext(TelegramSettings settings, IEnumerable<ICommand>? commands = null)
        {
            BotClient = Substitute.For<ITelegramBotClient>();
            BotWrapper = Substitute.For<ITelegramBotClientWrapper>();
            BotWrapper.Bot.Returns(BotClient);
            commands ??= [Substitute.For<ICommand>()];
            SettingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
            var logger = Substitute.For<ILogger<UpdateHandler>>();
            var statusHandlers = Enumerable.Empty<IConditionalStatusHandler>();
            var defaultStatusHandler = Substitute.For<IDefaultStatusHandler>();
            var updateHandler = Substitute.For<UpdateHandler>(BotWrapper, SettingsPersistenceLayer, statusHandlers, defaultStatusHandler, logger);
            var telegramSettings = Options.Create(settings);

            HostedTelegramBot = new HostedTelegramBot(
                BotWrapper,
                commands,
                updateHandler,
                telegramSettings,
                SettingsPersistenceLayer);
        }
    }
}
