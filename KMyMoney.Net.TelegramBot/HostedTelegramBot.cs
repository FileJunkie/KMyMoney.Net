using System.Security.Cryptography;
using KMyMoney.Net.TelegramBot.Commands;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Services;
using KMyMoney.Net.TelegramBot.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot;

public sealed class HostedTelegramBot(
    TelegramBotClientWrapper botWrapper,
    IEnumerable<ICommand> commands,
    UpdateHandler updateHandler,
    IOptions<TelegramSettings> telegramSettings,
    ISettingsPersistenceLayer settingsPersistenceLayer) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (telegramSettings.Value.WebhookUri != null)
        {
            var telegramSecret = RandomNumberGenerator.GetHexString(16);
            await settingsPersistenceLayer.SetSavedValueByKeyAsync(
                "telegramSecret",
                telegramSecret,
                cancellationToken: cancellationToken);
            await botWrapper.Bot.SetWebhook(
                url: telegramSettings.Value.WebhookUri.ToString(),
                secretToken: telegramSecret,
                cancellationToken: cancellationToken);
        }
        else
        {
            await botWrapper.Bot.DeleteWebhook(cancellationToken: cancellationToken);
            botWrapper.Bot.OnMessage += (message, type) => updateHandler.OnMessageAsync(message, type, cancellationToken);
            botWrapper.Bot.OnError += updateHandler.OnErrorAsync;
        }

        await botWrapper.Bot.SetMyCommands(
            commands.Select(c => new BotCommand(c.Command, c.Description)),
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await botWrapper.StopAsync();
    }
}