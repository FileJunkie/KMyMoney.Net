using KMyMoney.Net.TelegramBot.StatusHandlers;

namespace KMyMoney.Net.TelegramBot;

public static class ServiceConfigurationExtensions
{
    public static IServiceCollection AddStatusHandler<T>(this IServiceCollection services)
    where T : class, IConditionalStatusHandler => services
        .AddSingleton<T>()
        .AddSingleton<IConditionalStatusHandler>(sp => sp.GetRequiredService<T>());
}