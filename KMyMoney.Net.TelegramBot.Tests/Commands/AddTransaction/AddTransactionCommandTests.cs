using KMyMoney.Net.Core;
using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.TelegramBot.Commands.AddTransaction;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using KMyMoney.Net.Tests.Common;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Tests.Commands.AddTransaction;

public class AddTransactionCommandTests
{
    [Fact]
    public async Task HandleAsync_ShouldPromptForFromAccount_WhenFileIsLoaded()
    {
        // Arrange
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var addTransactionFromAccountHandler =
            new AddTransactionFromAccountHandler(
                null!, null!, null!, null!);
        var fileLoader = Substitute.For<IFileLoader>();
        var command = new AddTransactionCommand(settingsPersistenceLayer,
            botWrapper, addTransactionFromAccountHandler, fileLoader);

        var message = new Message
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };
        var kmyFile = new KMyMoneyFile(new Uri("file:///test.kmy"),
            Substitute.For<IFileAccessor>(),
            TestUtils.CreateTestKmyMoneyFileRoot());

        fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None)
            .Returns(kmyFile);

        // Act
        await command.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(
                r => r.Text.Contains("Choose account to take money from")),
            CancellationToken.None);
    }
}