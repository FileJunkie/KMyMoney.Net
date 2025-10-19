using KMyMoney.Net.Core;
using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.TelegramBot.Commands.AddTransaction;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using KMyMoney.Net.Tests.Common;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Tests.Commands.AddTransaction;

public class AddTransactionFromAccountHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldPromptForToAccount_WhenFromAccountIsValid()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var fileLoader = Substitute.For<IFileLoader>();
        var handler = new AddTransactionFromAccountHandler(botWrapper,
            settingsPersistenceLayer, fileLoader);

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
            UserSettings.AccountFrom, "Checking Account",
            cancellationToken: CancellationToken.None);
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(
                r => r.Text.Contains("Choose account to put money into")),
            CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldAbort_WhenFromAccountIsInvalid()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var fileLoader = Substitute.For<IFileLoader>();
        var handler = new AddTransactionFromAccountHandler(botWrapper,
            settingsPersistenceLayer, fileLoader);

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
}
