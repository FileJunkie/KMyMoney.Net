using KMyMoney.Net.TelegramBot.Persistence.Etcd;
using KMyMoney.Net.TelegramBot.Persistence.InMemory;
using KMyMoney.Net.TelegramBot.Settings;
using KMyMoney.Net.TelegramBot.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;

namespace KMyMoney.Net.TelegramBot.Tests;

public sealed class ServiceConfigurationTests
{
    private static readonly IServiceScope Scope;
    private static readonly Type[] ServiceTypes;

    static ServiceConfigurationTests()
    {
        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"Telegram:{nameof(TelegramSettings.ApiToken)}"] = "value"
            })
            .Build();

        services
            .AddInMemoryPersistenceLayer()
            .AddSingleton(Substitute.For<ITelegramBotClientWrapper>())
            .ConfigureServices(configuration);

        var serviceProvider = services.BuildServiceProvider();
        Scope = serviceProvider.CreateScope();
        ServiceTypes = services
            .Select(s => s.ServiceType)
            .Where(s => !s.IsGenericType)
            .Distinct()
            .ToArray();
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void TryGettingService(Type type)
    {
        var service = Scope.ServiceProvider.GetRequiredService(type);
        service.ShouldNotBeNull();
    }

    private class TestData : TheoryData<Type>
    {
        public TestData()
        {
            AddRange(ServiceTypes);
        }
    }
}