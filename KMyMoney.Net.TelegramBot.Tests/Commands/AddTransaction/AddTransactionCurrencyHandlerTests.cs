using KMyMoney.Net.TelegramBot.Commands.AddTransaction;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Tests.Commands.AddTransaction;

public class AddTransactionCurrencyHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldPromptForPrice_WhenCurrencyIsProvided()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var addTransactionPriceHandler =
            new AddTransactionPriceHandler(null!, null!, null!);
        var handler = new AddTransactionCurrencyHandler(botWrapper,
            settingsPersistenceLayer, addTransactionPriceHandler);

        var message = new Message
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456 }, Text = "USD" };

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await settingsPersistenceLayer.Received(1).SetUserSettingByUserIdAsync(123,
            UserSettings.Currency, "USD", cancellationToken: CancellationToken.None);
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(
                r => r.Text.Contains("Enter transaction amount")),
            CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldAbort_WhenCurrencyIsNotProvided()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var addTransactionPriceHandler =
            new AddTransactionPriceHandler(null!, null!, null!);
        var handler = new AddTransactionCurrencyHandler(botWrapper,
            settingsPersistenceLayer, addTransactionPriceHandler);

        var message = new Message
            { From = new User { Id = 123 }, Chat = new Chat { Id = 456 }, Text = "" };

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(r => r.Text.Contains("Currency not chosen")),
            CancellationToken.None);
    }
}