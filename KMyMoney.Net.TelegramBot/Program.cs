using KMyMoney.Net.TelegramBot.Commands;
using KMyMoney.Net.TelegramBot.Persistence.InMemory;
using KMyMoney.Net.TelegramBot.Settings;
using TgBotFramework;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<DropboxSettings>()
    .Bind(builder.Configuration.GetSection("Dropbox"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.
    AddOptions<BotSettings>()
    .Bind(builder.Configuration.GetSection("Telegram"))
    .ValidateDataAnnotations()
    .ValidateOnStart()
    .Services
    .AddBotService<BaseBot, UpdateContext>(botBuilder => botBuilder
        .UseLongPolling()
        .SetPipeline(pipelineBuilder => pipelineBuilder
            .UseCommand<LoginCommand>("login")
            .UseCommand<LoginCodeCommand>("logincode")
            .UseCommand<FileCommand>("file")
            .UseCommand<AccountsCommand>("accounts")));

builder.Services
    .AddSingleton<LoginCommand>()
    .AddSingleton<LoginCodeCommand>()
    .AddSingleton<FileCommand>()
    .AddSingleton<AccountsCommand>();

// TODO local testing only 
builder.Services.AddInMemoryPersistenceLayer();

builder.Services.AddSystemd();

var app = builder.Build();

await app.RunAsync();
