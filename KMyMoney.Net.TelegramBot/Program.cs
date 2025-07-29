using KMyMoney.Net.TelegramBot;
using KMyMoney.Net.TelegramBot.Persistence.InMemory;
using KMyMoney.Net.TelegramBot.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<DropboxSettings>()
    .Bind(builder.Configuration.GetSection("Dropbox"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<TelegramSettings>()
    .Bind(builder.Configuration.GetSection("Telegram"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton(sp =>
{
    var telegramSettings = sp.GetRequiredService<IOptions<TelegramSettings>>();
    return new TelegramBotClient(telegramSettings.Value.ApiToken);
});

// TODO local testing only 
builder.Services.AddInMemoryPersistenceLayer();
builder.Services.AddHostedService<TelegramService>();

var app = builder.Build();

await app.RunAsync();
