using KMyMoney.Net.Core;
using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.Models;
using KMyMoney.Net.TelegramBot.Commands;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using TelegramUser = Telegram.Bot.Types.User;

namespace KMyMoney.Net.TelegramBot.Tests.Commands;

public class AccountCommandTests
{
    [Fact]
    public async Task HandleAsync_ShouldSendAccountList_WhenFileIsLoaded()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var fileLoader = Substitute.For<IFileLoader>();
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var command = new AccountsCommand(botWrapper, settingsPersistenceLayer, fileLoader);

        var message = new Message { From = new TelegramUser { Id = 123 }, Chat = new Chat { Id = 456 } };
        var kmyFile = new KMyMoneyFile(new Uri("file:///test.kmy"), Substitute.For<IFileAccessor>(), CreateTestKmyMoneyFileRootWithData());

        fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None).Returns(kmyFile);

        // Act
        await command.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(Arg.Is<SendMessageRequest>(r => r.Text.Contains("Checking Account")), CancellationToken.None);
        await settingsPersistenceLayer.Received(1).SetUserSettingByUserIdAsync(
            123,
            UserSettings.Status,
            null,
            cancellationToken: CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldDoNothing_WhenFileLoaderReturnsNull()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var fileLoader = Substitute.For<IFileLoader>();
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var command = new AccountsCommand(botWrapper, settingsPersistenceLayer, fileLoader);

        var message = new Message { From = new TelegramUser { Id = 123 }, Chat = new Chat { Id = 456 } };

        fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None).Returns((KMyMoneyFile?)null);

        // Act
        await command.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.DidNotReceiveWithAnyArgs().SendRequest(Arg.Any<IRequest<Message>>(), CancellationToken.None);
        await settingsPersistenceLayer.Received(1).SetUserSettingByUserIdAsync(
            123,
            UserSettings.Status,
            null,
            cancellationToken: CancellationToken.None);
    }

    private static KmyMoneyFileRoot CreateTestKmyMoneyFileRootWithData() => new()
    {
        FileInfo = new(),
        User = new()
        {
            Name = "test-user",
            Email = "test@email.com"
        },
        Institutions = new() { Values = [] },
        Payees = new() { Values = [] },
        CostCenters = new(),
        Tags = new(),
        Accounts = new()
        {
            Values =
            [
                new()
                {
                    Id = "A000001",
                    Name = "Checking Account",
                    Type = "Asset",
                    Currency = "USD"
                }
            ]
        },
        Transactions = new() { Values = [] },
        KeyValuePairs = new() { Pair = [] },
        Schedules = new() { Values = [] },
        Securities = new() { Values = [] },
        Currencies = new() { Values = [] },
        Prices = new() { Values = [] },
        Reports = new() { Values = [] },
        Budgets = new(),
        OnlineJobs = new()
    };
}