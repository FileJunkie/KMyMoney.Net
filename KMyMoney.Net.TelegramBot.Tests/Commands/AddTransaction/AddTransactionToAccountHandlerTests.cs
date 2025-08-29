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

public class AddTransactionToAccountHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldPromptForCurrency_WhenToAccountIsValid()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var addTransactionCurrencyHandler = new AddTransactionCurrencyHandler(
            Substitute.For<ITelegramBotClientWrapper>(),
            Substitute.For<ISettingsPersistenceLayer>(),
            new AddTransactionPriceHandler(null!, null!, null!),
            Substitute.For<IFileLoader>());
        var fileLoader = Substitute.For<IFileLoader>();
        var handler = new AddTransactionToAccountHandler(botWrapper,
            settingsPersistenceLayer, addTransactionCurrencyHandler, fileLoader);

        var message = new Message
        {
            From = new User { Id = 123 },
            Chat = new Chat { Id = 456 },
            Text = "Checking Account"
        };
        var kmyFile = new KMyMoneyFile(new Uri("file:///test.kmy"),
            Substitute.For<IFileAccessor>(),
            TestUtils.CreateTestKmyMoneyFileRoot());

        fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None)
            .Returns(kmyFile);

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await settingsPersistenceLayer.Received(1).SetUserSettingByUserIdAsync(123,
            UserSettings.AccountTo, "Checking Account",
            cancellationToken: CancellationToken.None);
        await settingsPersistenceLayer.Received(1).SetUserSettingByUserIdAsync(123,
            UserSettings.Status, "AddTransactionEnteringCurrency",
            cancellationToken: CancellationToken.None);
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(r => r.Text.Contains("Choose currency")),
            CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldAbort_WhenToAccountIsInvalid()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var addTransactionCurrencyHandler = new AddTransactionCurrencyHandler(
            Substitute.For<ITelegramBotClientWrapper>(),
            Substitute.For<ISettingsPersistenceLayer>(),
            new AddTransactionPriceHandler(null!, null!, null!),
            Substitute.For<IFileLoader>());
        var fileLoader = Substitute.For<IFileLoader>();
        var handler = new AddTransactionToAccountHandler(botWrapper,
            settingsPersistenceLayer, addTransactionCurrencyHandler, fileLoader);

        var message = new Message
        {
            From = new User { Id = 123 },
            Chat = new Chat { Id = 456 },
            Text = "Invalid Account"
        };
        var kmyFile = new KMyMoneyFile(new Uri("file:///test.kmy"),
            Substitute.For<IFileAccessor>(),
            TestUtils.CreateTestKmyMoneyFileRoot());

        fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None)
            .Returns(kmyFile);

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(r => r.Text.Contains("Wrong account, aborting")),
            CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldAbort_WhenFileIsNotLoaded()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var addTransactionCurrencyHandler = new AddTransactionCurrencyHandler(
            Substitute.For<ITelegramBotClientWrapper>(),
            Substitute.For<ISettingsPersistenceLayer>(),
            new AddTransactionPriceHandler(null!, null!, null!),
            Substitute.For<IFileLoader>());
        var fileLoader = Substitute.For<IFileLoader>();
        var handler = new AddTransactionToAccountHandler(botWrapper,
            settingsPersistenceLayer, addTransactionCurrencyHandler, fileLoader);

        var message = new Message
        {
            From = new User { Id = 123 },
            Chat = new Chat { Id = 456 },
            Text = "Checking Account"
        };

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