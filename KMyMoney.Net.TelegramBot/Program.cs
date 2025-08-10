using System.Diagnostics.CodeAnalysis;
using KMyMoney.Net.TelegramBot;
using KMyMoney.Net.TelegramBot.Persistence.Etcd;
using KMyMoney.Net.TelegramBot.Telegram;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .ConfigureServices(builder.Configuration)
    .AddControllers();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();
await app.RunAsync();

[ExcludeFromCodeCoverage(Justification = "Program class, nothing to test")]
public partial class Program { }