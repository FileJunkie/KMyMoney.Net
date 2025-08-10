using KMyMoney.Net.TelegramBot.Commands;
using KMyMoney.Net.TelegramBot.Commands.AddTransaction;
using KMyMoney.Net.TelegramBot.Commands.File;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence.Etcd;
using KMyMoney.Net.TelegramBot.Services;
using KMyMoney.Net.TelegramBot.Settings;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using KMyMoney.Net.TelegramBot.Telegram;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Telegram.Bot.AspNetCore;

namespace KMyMoney.Net.TelegramBot;

public static class ServiceConfigurationExtensions
{
    public static IServiceCollection ConfigureServices(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .ConfigureSettings(configuration)
            .ConfigurePersistenceLayer()
            .ConfigureCommandsAndHandlers()
            .ConfigureTelegram()
            .ConfigureDropbox()
            .ConfigureSystem();

    private static IServiceCollection ConfigureSettings(
        this IServiceCollection services,
        IConfiguration configuration) => services
        .AddOptions<DropboxSettings>()
        .Bind(configuration.GetSection("Dropbox"))
        .ValidateDataAnnotations()
        .ValidateOnStart()
        .Services
        .AddOptions<TelegramSettings>()
        .Bind(configuration.GetSection("Telegram"))
        .ValidateDataAnnotations()
        .ValidateOnStart()
        .Services
        .AddOptions<EtcdSettings>()
        .Bind(configuration.GetSection("Etcd"))
        .ValidateDataAnnotations()
        .ValidateOnStart()
        .Services;

    private static IServiceCollection ConfigurePersistenceLayer(this IServiceCollection services)
        => services.TryAddEtcdPersistenceLayer();

    private static IServiceCollection ConfigureCommandsAndHandlers(this IServiceCollection services) => services
        .AddSingleton<IDefaultStatusHandler, DefaultStatusHandler>()
        .AddSingleton<ICommand, LoginCommand>()
        .AddSingleton<ICommand, FileCommand>()
        .AddStatusHandler<FileEntryStatusHandler>()
        .AddSingleton<ICommand, AccountsCommand>()
        .AddSingleton<ICommand, AddTransactionCommand>()
        .AddStatusHandler<AddTransactionFromAccountHandler>()
        .AddStatusHandler<AddTransactionToAccountHandler>()
        .AddStatusHandler<AddTransactionCurrencyHandler>()
        .AddStatusHandler<AddTransactionPriceHandler>();

    private static IServiceCollection ConfigureTelegram(this IServiceCollection services)
    {
        services.TryAddSingleton<ITelegramBotClientWrapper, TelegramBotClientWrapper>();

        return services
            .AddHostedService<HostedTelegramBot>()
            .AddSingleton<IUpdateHandler, UpdateHandler>()
            .ConfigureTelegramBotMvc();
    }

    private static IServiceCollection ConfigureDropbox(this IServiceCollection services) => services
        .AddSingleton<IDropboxOAuth2HelperWrapper, DropboxOAuth2HelperWrapper>()
        .AddSingleton<IFileAccessorFactory, DropboxFileAccessorFactory>()
        .AddSingleton<IFileLoader, FileLoader>();

    private static IServiceCollection ConfigureSystem(this IServiceCollection services) => services
        .AddSystemd()
        .AddLogging();

    private static IServiceCollection AddStatusHandler<T>(this IServiceCollection services)
    where T : class, IConditionalStatusHandler => services
        .AddSingleton<T>()
        .AddSingleton<IConditionalStatusHandler>(sp => sp.GetRequiredService<T>());
}