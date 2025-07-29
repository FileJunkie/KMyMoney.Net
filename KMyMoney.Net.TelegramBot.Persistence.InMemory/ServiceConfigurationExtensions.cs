using Microsoft.Extensions.DependencyInjection;

namespace KMyMoney.Net.TelegramBot.Persistence.InMemory;

public static class ServiceConfigurationExtensions
{
    public static IServiceCollection AddInMemoryPersistenceLayer(this IServiceCollection services) =>
        services.AddSingleton<ISettingsPersistenceLayer, InMemoryPersistenceLayer>();
}