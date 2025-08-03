using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using dotnet_etcd;
using dotnet_etcd.interfaces;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace KMyMoney.Net.TelegramBot.Persistence.Etcd;

public static class ServiceConfigurationExtensions
{
    public static IServiceCollection AddEtcdPersistenceLayer(this IServiceCollection services) => services
        .AddSingleton<IEtcdClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<EtcdSettings>>().Value;
            using var certificate = X509CertificateLoader.LoadCertificateFromFile(settings.Certificate);
            using var key = RSA.Create();
            key.ImportFromPem(File.ReadAllText(settings.Key));
            var certificatePair = certificate.CopyWithPrivateKey(key);
            var clientCertificateCollection = new X509CertificateCollection { certificatePair };
            var handler = new SocketsHttpHandler
            {
                KeepAlivePingDelay = TimeSpan.FromSeconds(30),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
                SslOptions = new()
                {
                    ClientCertificates = clientCertificateCollection,
                    RemoteCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                    {
                        if (sslPolicyErrors == SslPolicyErrors.None)
                        {
                            return true;
                        }

                        if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) == 0
                            || chain == null
                            || cert == null)
                        {
                            return false;
                        }

                        var rootCert = X509CertificateLoader.LoadCertificateFromFile(settings.RootCertificate);
                        chain.ChainPolicy.CustomTrustStore.Add(rootCert);
                        chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                        return chain.Build(new X509Certificate2(cert));
                    }
                }
            };

            return new EtcdClient(connectionString:
                settings.ConnectionString,
                configureChannelOptions: opts =>
                {
                    opts.Credentials = ChannelCredentials.SecureSsl;
                    opts.HttpHandler = handler;
                });
        })
        .AddSingleton<ISettingsPersistenceLayer, EtcdSettingsPersistenceLayer>();
}
