using KMyMoney.Net.Core;
using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.TelegramBot.Commands.AddTransaction;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using KMyMoney.Net.Tests.Common;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Tests.Commands.AddTransaction;

public class AddTransactionPriceHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldSaveTransaction_WhenPriceIsValid()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var fileLoader = Substitute.For<IFileLoader>();
        var handler = new AddTransactionPriceHandler(botWrapper,
            settingsPersistenceLayer, fileLoader);

        var message = new Message
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456 }, Text = "123.45" };
        var kmyMoneyFileRoot = TestUtils.CreateTestKmyMoneyFileRoot();
        var kmyFile = new KMyMoneyFile(new Uri("file:///test.kmy"),
            Substitute.For<IFileAccessor>(),
            kmyMoneyFileRoot);

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.AccountFrom)
            .Returns("A000001");
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.AccountTo)
            .Returns("A000001"); // Using same account for simplicity
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.Currency)
            .Returns("USD");
        fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None)
            .Returns(kmyFile);

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(r => r.Text.Contains("Saved")),
            CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldSendError_WhenPriceIsInvalid()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var fileLoader = Substitute.For<IFileLoader>();
        var handler = new AddTransactionPriceHandler(botWrapper,
            settingsPersistenceLayer, fileLoader);

        var message = new Message
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456 }, Text = "invalid" };

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.AccountFrom)
            .Returns("A1");
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.AccountTo)
            .Returns("A2");
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.Currency)
            .Returns("USD");

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(
                r => r.Text.Contains("What kind of amount is that?")),
            CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldSendError_WhenAccountFromIsNull()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var fileLoader = Substitute.For<IFileLoader>();
        var handler = new AddTransactionPriceHandler(botWrapper,
            settingsPersistenceLayer, fileLoader);

        var message = new Message
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456 }, Text = "123.45" };

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.AccountFrom)
            .Returns((string?)null);

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(r => r.Text.Contains("AccountFrom was somehow null?")),
            CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldSendError_WhenAccountToIsNull()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var fileLoader = Substitute.For<IFileLoader>();
        var handler = new AddTransactionPriceHandler(botWrapper,
            settingsPersistenceLayer, fileLoader);

        var message = new Message
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456 }, Text = "123.45" };

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.AccountFrom)
            .Returns("A1");
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.AccountTo)
            .Returns((string?)null);

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(r => r.Text.Contains("AccountTo was somehow null?")),
            CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldSendError_WhenCurrencyIsNull()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var fileLoader = Substitute.For<IFileLoader>();
        var handler = new AddTransactionPriceHandler(botWrapper,
            settingsPersistenceLayer, fileLoader);

        var message = new Message
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456 }, Text = "123.45" };

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.AccountFrom)
            .Returns("A1");
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.AccountTo)
            .Returns("A2");
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.Currency)
            .Returns((string?)null);

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(r => r.Text.Contains("Currency was somehow null?")),
            CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldDoNothing_WhenFileIsNotLoaded()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var fileLoader = Substitute.For<IFileLoader>();
        var handler = new AddTransactionPriceHandler(botWrapper,
            settingsPersistenceLayer, fileLoader);

        var message = new Message
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456 }, Text = "123.45" };

        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.AccountFrom)
            .Returns("A1");
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.AccountTo)
            .Returns("A2");
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(123, UserSettings.Currency)
            .Returns("USD");
        fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None)
            .Returns((KMyMoneyFile?)null);

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.DidNotReceiveWithAnyArgs().SendRequest(
            Arg.Any<SendMessageRequest>(),
            CancellationToken.None);
    }
}