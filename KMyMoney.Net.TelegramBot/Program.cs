using KMyMoney.Net.TelegramBot;
using KMyMoney.Net.TelegramBot.Persistence.InMemory;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(
    new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_API_TOKEN")!));
// TODO local testing only 
builder.Services.AddInMemoryPersistenceLayer();
builder.Services.AddHostedService<TelegramService>();

var app = builder.Build();

app.UseHttpsRedirection();

await app.RunAsync();
