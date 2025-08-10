using KMyMoney.Net.TelegramBot;

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