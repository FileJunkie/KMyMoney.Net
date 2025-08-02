using KMyMoney.Net.TelegramBot.StatusHandlers;

namespace KMyMoney.Net.TelegramBot;

public static class ServiceConfigurationExtensions
{
    public static IServiceCollection AddStatusHandler<T>(this IServiceCollection services)
    where T : class, IStatusHandler => services
        .AddSingleton<T>()
        .AddSingleton<IStatusHandler>(sp => sp.GetRequiredService<T>());
}