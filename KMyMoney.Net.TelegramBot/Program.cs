using KMyMoney.Net.TelegramBot;
using KMyMoney.Net.TelegramBot.Commands;
using KMyMoney.Net.TelegramBot.Commands.AddTransaction;
using KMyMoney.Net.TelegramBot.Commands.File;
using KMyMoney.Net.TelegramBot.Commands.Login;
using KMyMoney.Net.TelegramBot.Persistence.Etcd;
using KMyMoney.Net.TelegramBot.Persistence.InMemory;
using KMyMoney.Net.TelegramBot.Settings;
using KMyMoney.Net.TelegramBot.StatusHandlers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<DropboxSettings>()
    .Bind(builder.Configuration.GetSection("Dropbox"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<TelegramSettings>()
    .Bind(builder.Configuration.GetSection("Telegram"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<EtcdSettings>()
    .Bind(builder.Configuration.GetSection("Etcd"))
    .ValidateDataAnnotations()
    .ValidateOnStart()
    .Services
    .AddEtcdPersistenceLayer();

builder.Services
    .AddSingleton<IDefaultStatusHandler, DefaultStatusHandler>()
    .AddSingleton<ICommand, LoginCommand>()
    .AddStatusHandler<LoginCodeEntryStatusHandler>()
    .AddSingleton<ICommand, FileCommand>()
    .AddStatusHandler<FileEntryStatusHandler>()
    .AddSingleton<ICommand, AccountsCommand>()
    .AddSingleton<ICommand, AddTransactionCommand>()
    .AddStatusHandler<AddTransactionFromAccountHandler>()
    .AddStatusHandler<AddTransactionToAccountHandler>()
    .AddStatusHandler<AddTransactionCurrencyHandler>()
    .AddStatusHandler<AddTransactionPriceHandler>();

builder.Services.AddSystemd();

builder.Services
    .AddSingleton<TelegramBotClientWrapper>()
    .AddHostedService<HostedTelegramBot>();

builder.Services.AddLogging();

var app = builder.Build();

await app.RunAsync();
