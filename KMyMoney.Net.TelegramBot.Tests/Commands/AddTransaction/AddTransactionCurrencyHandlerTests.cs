using KMyMoney.Net.Core;
using KMyMoney.Net.Core.FileAccessors;
using KMyMoney.Net.Models;
using KMyMoney.Net.TelegramBot.Commands.AddTransaction;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using NSubstitute;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using FileInfo = KMyMoney.Net.Models.FileInfo;
using User = KMyMoney.Net.Models.User;
using TelegramUser = Telegram.Bot.Types.User;

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
        var fileLoader = Substitute.For<IFileLoader>();
        var handler = new AddTransactionCurrencyHandler(botWrapper,
            settingsPersistenceLayer, addTransactionPriceHandler, fileLoader);

        var message = new Message
            { From = new TelegramUser { Id = 123 }, Chat = new Chat { Id = 456 }, Text = "USD" };

        var kmyMoneyFile = new KMyMoneyFile(new Uri("file:///test.kmy"), Substitute.For<IFileAccessor>(),
            new KmyMoneyFileRoot
            {
                Prices = new Prices
                {
                    Values = new []
                    {
                        new PricePair
                        {
                            From = "USD",
                            To = "EUR",
                            Price = []
                        }
                    }
                },
                Accounts = new Accounts { Values = []},
                Budgets = new Budgets(),
                CostCenters = new CostCenters(),
                Currencies = new Currencies { Values = []},
                FileInfo = new FileInfo(),
                Institutions = new Institutions { Values = []},
                KeyValuePairs = new KeyValuePairs { Pair = []},
                OnlineJobs = new OnlineJobs(),
                Payees = new Payees { Values = []},
                Reports = new Reports { Values = []},
                Schedules = new Schedules { Values = []},
                Securities = new Securities { Values = []},
                Tags = new Tags(),
                Transactions = new Transactions { Values = []},
                User = new User { Email = "", Name = "" }
            });

        fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None)
            .Returns(kmyMoneyFile);

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
        var fileLoader = Substitute.For<IFileLoader>();
        var handler = new AddTransactionCurrencyHandler(botWrapper,
            settingsPersistenceLayer, addTransactionPriceHandler, fileLoader);

        var message = new Message
            { From = new TelegramUser { Id = 123 }, Chat = new Chat { Id = 456 }, Text = "" };

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(r => r.Text.Contains("Currency not chosen")),
            CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldSendErrorMessage_WhenCurrencyIsInvalid()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var addTransactionPriceHandler =
            new AddTransactionPriceHandler(null!, null!, null!);
        var fileLoader = Substitute.For<IFileLoader>();
        var handler = new AddTransactionCurrencyHandler(botWrapper,
            settingsPersistenceLayer, addTransactionPriceHandler, fileLoader);

        var message = new Message
            { From = new TelegramUser { Id = 123 }, Chat = new Chat { Id = 456 }, Text = "RUB" };

        var kmyMoneyFile = new KMyMoneyFile(new Uri("file:///test.kmy"), Substitute.For<IFileAccessor>(),
            new KmyMoneyFileRoot
            {
                Prices = new Prices
                {
                    Values = new []
                    {
                        new PricePair
                        {
                            From = "USD",
                            To = "EUR",
                            Price = []
                        }
                    }
                },
                Accounts = new Accounts { Values = []},
                Budgets = new Budgets(),
                CostCenters = new CostCenters(),
                Currencies = new Currencies { Values = []},
                FileInfo = new FileInfo(),
                Institutions = new Institutions { Values = []},
                KeyValuePairs = new KeyValuePairs { Pair = []},
                OnlineJobs = new OnlineJobs(),
                Payees = new Payees { Values = []},
                Reports = new Reports { Values = []},
                Schedules = new Schedules { Values = []},
                Securities = new Securities { Values = []},
                Tags = new Tags(),
                Transactions = new Transactions { Values = []},
                User = new User { Email = "", Name = "" }
            });

        fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None)
            .Returns(kmyMoneyFile);

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(r => r.Text.Contains("Wrong currency")),
            CancellationToken.None);
    }

    [Fact]
    public async Task HandleAsync_ShouldDoNothing_WhenFileLoaderFails()
    {
        // Arrange
        var botClient = Substitute.For<ITelegramBotClient>();
        var botWrapper = Substitute.For<ITelegramBotClientWrapper>();
        botWrapper.Bot.Returns(botClient);
        var settingsPersistenceLayer = Substitute.For<ISettingsPersistenceLayer>();
        var addTransactionPriceHandler =
            new AddTransactionPriceHandler(null!, null!, null!);
        var fileLoader = Substitute.For<IFileLoader>();
        var handler = new AddTransactionCurrencyHandler(botWrapper,
            settingsPersistenceLayer, addTransactionPriceHandler, fileLoader);

        var message = new Message
            { From = new TelegramUser { Id = 123 }, Chat = new Chat { Id = 456 }, Text = "USD" };

        fileLoader.LoadKMyMoneyFileOrSendErrorAsync(message, CancellationToken.None)
            .Returns(Task.FromResult<KMyMoneyFile?>(null));

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        await botClient.DidNotReceive().SendRequest(
            Arg.Any<SendMessageRequest>(),
            CancellationToken.None);
    }
}