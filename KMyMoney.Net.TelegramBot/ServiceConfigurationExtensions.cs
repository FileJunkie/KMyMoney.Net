using KMyMoney.Net.TelegramBot.Commands;
using KMyMoney.Net.TelegramBot.Commands.AddTransaction;
using KMyMoney.Net.TelegramBot.Commands.File;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.FileAccess;
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
            .ConfigurePersistenceLayer(configuration)
            .ConfigureCommandsAndHandlers()
            .ConfigureTelegram(configuration)
            .ConfigureStorage(configuration)
            .ConfigureSystem();

    private static IServiceCollection ConfigurePersistenceLayer(
        this IServiceCollection services, IConfiguration configuration)
        => services
            .AddOptions<EtcdSettings>()
            .Bind(configuration.GetSection("Etcd"))
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Services
            .TryAddEtcdPersistenceLayer();

    private static IServiceCollection ConfigureCommandsAndHandlers(this IServiceCollection services) => services
        .AddSingleton<IDefaultStatusHandler, DefaultStatusHandler>()
        .AddSingleton<ICommand, AccountsCommand>()
        .AddSingleton<ICommand, AddTransactionCommand>()
        .AddStatusHandler<AddTransactionFromAccountHandler>()
        .AddStatusHandler<AddTransactionToAccountHandler>()
        .AddStatusHandler<AddTransactionCurrencyHandler>()
        .AddStatusHandler<AddTransactionPriceHandler>()
        .AddSingleton<ICommand, LoginCommand>()
        .AddSingleton<ICommand, FileCommand>()
        .AddStatusHandler<FileEntryStatusHandler>();

    private static IServiceCollection ConfigureTelegram(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.TryAddSingleton<ITelegramBotClientWrapper, TelegramBotClientWrapper>();

        return services
            .AddOptions<TelegramSettings>()
            .Bind(configuration.GetSection("Telegram"))
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Services
            .AddHostedService<HostedTelegramBot>()
            .AddSingleton<IUpdateHandler, UpdateHandler>()
            .ConfigureTelegramBotMvc();
    }

    private static IServiceCollection ConfigureStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var configurationSection = configuration.GetSection("Dropbox");
        if (configurationSection.Exists())
        {
            services = services.ConfigureDropbox(configurationSection);
        }

        return services.AddSingleton<IFileLoader, FileLoader>();
    }

    private static IServiceCollection ConfigureDropbox(
        this IServiceCollection services,
        IConfigurationSection configurationSection) => services
        .AddOptions<DropboxSettings>()
        .Bind(configurationSection)
        .ValidateDataAnnotations()
        .ValidateOnStart()
        .Services
        .AddSingleton<IDropboxOAuth2HelperWrapper, DropboxOAuth2HelperWrapper>()
        .AddSingleton<IFileAccessService, DropboxFileAccessService>();

    private static IServiceCollection ConfigureSystem(this IServiceCollection services) => services
        .AddSystemd()
        .AddLogging();

    private static IServiceCollection AddStatusHandler<T>(this IServiceCollection services)
    where T : class, IConditionalStatusHandler => services
        .AddSingleton<T>()
        .AddSingleton<IConditionalStatusHandler>(sp => sp.GetRequiredService<T>());
}