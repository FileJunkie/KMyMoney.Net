using dotnet_etcd.DependencyInjection;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;

namespace KMyMoney.Net.TelegramBot.Persistence.Etcd;

public static class ServiceConfigurationExtensions
{
    public static IServiceCollection AddEtcdPersistenceLayer(this IServiceCollection services)
    {
        return services.AddEtcdClient(options =>
        {
            // TODO use options
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
    }
}