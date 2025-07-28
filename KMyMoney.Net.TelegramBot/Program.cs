using dotnet_etcd.DependencyInjection;
using Grpc.Core;
using KMyMoney.Net.TelegramBot;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(
    new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_API_TOKEN")!));
builder.Services.AddEtcdClient(options =>
{
    options.ConnectionString = Environment.GetEnvironmentVariable("ETCD_CONNECTION_STRING")!;

    options.ConfigureChannel = channelOptions => {
        channelOptions.Credentials = new SslCredentials(
            rootCertificates: File.ReadAllText(Environment.GetEnvironmentVariable("ETCD_CACERT")!),
            keyCertificatePair: new KeyCertificatePair(
                File.ReadAllText(Environment.GetEnvironmentVariable("ETCD_CERT")!),
                File.ReadAllText(Environment.GetEnvironmentVariable("ETCD_KEY")!)
            ));
    };
});
builder.Services.AddHostedService<TelegramTryout>();

var app = builder.Build();

app.UseHttpsRedirection();

await app.RunAsync();
