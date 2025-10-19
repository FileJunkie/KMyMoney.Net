using KMyMoney.Net.TelegramBot.Common;
using KMyMoney.Net.TelegramBot.Exceptions;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using NSubstitute;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Tests.Common;

public class AbstractMessageHandlerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Status_ProcessedCorrectly(bool keepStatus)
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClientWrapper>();
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var exception = new WithUserMessageException("Message", keepStatus);
        var handler = new AbstractMessageHandlerMock(exception, botClient, settingsPersistenceLayer);

        var message = new Message { From = new User { Id = 123 }, Chat = new Chat { Id = 456 } };
        settingsPersistenceLayer.GetUserSettingByUserIdAsync(
                message.From!.Id,
                UserSettings.Status,
                Arg.Any<CancellationToken>())
            .Returns("status");

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await settingsPersistenceLayer.Received(1).GetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            Arg.Any<CancellationToken>());
        await settingsPersistenceLayer.Received(1).SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            null,
            Arg.Any<TimeSpan?>(),
            Arg.Any<CancellationToken>());
        await settingsPersistenceLayer.Received(keepStatus ? 1 : 0).SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            "status",
            Arg.Any<TimeSpan?>(),
            Arg.Any<CancellationToken>());
    }

    private class AbstractMessageHandlerMock(
        Exception exception,
        ITelegramBotClientWrapper botClient,
        ISettingsPersistenceLayer settingsPersistenceLayer) :
        AbstractMessageHandler(botClient, settingsPersistenceLayer)
    {
        protected override Task HandleAfterResettingStatusAsync(Message message, CancellationToken cancellationToken)
        {
            throw exception;
        }
    }
}
