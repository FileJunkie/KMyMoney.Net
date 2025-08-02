using KMyMoney.Net.TelegramBot;
using KMyMoney.Net.TelegramBot.Commands;
using KMyMoney.Net.TelegramBot.Persistence.InMemory;
using KMyMoney.Net.TelegramBot.Settings;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.StatusHandlers.Login;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<DropboxSettings>()
    .Bind(builder.Configuration.GetSection("Dropbox"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<TelegramSettings>()
    .Bind(builder.Configuration.GetSection("Telegram"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddSingleton<IDefaultStatusHandler, DefaultStatusHandler>()
    .AddSingleton<ICommand, LoginCommand>()
    .AddSingleton<IConditionalStatusHandler, LoginCodeEntryStatusHandler>()
    .AddStatusHandler<LoginCodeEntryStatusHandler>()
    .AddSingleton<ICommand, FileCommand>()
    .AddSingleton<ICommand, AccountsCommand>();

// TODO local testing only 
builder.Services.AddInMemoryPersistenceLayer();

builder.Services.AddSystemd();

builder.Services
    .AddSingleton<TelegramBotClientWrapper>()
    .AddHostedService<HostedTelegramBot>();

builder.Services.AddLogging();

var app = builder.Build();

await app.RunAsync();
