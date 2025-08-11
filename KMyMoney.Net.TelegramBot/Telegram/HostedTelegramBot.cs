using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using KMyMoney.Net.TelegramBot.Commands;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Services;
using KMyMoney.Net.TelegramBot.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Telegram;

public sealed class HostedTelegramBot(
    ITelegramBotClientWrapper botWrapper,
    IEnumerable<ICommand> commands,
    IUpdateHandler updateHandler,
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
            SetupEventHandling(cancellationToken);
        }

        await botWrapper.Bot.SetMyCommands(
            commands.Select(c => new BotCommand(c.Command, c.Description)),
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await botWrapper.StopAsync();
    }

    [ExcludeFromCodeCoverage(Justification = "Only testable on real telegram bot client")]
    private void SetupEventHandling(CancellationToken cancellationToken)
    {
        if (botWrapper.Bot is TelegramBotClient telegramBot)
        {
            telegramBot.OnMessage += (message, type) =>
                updateHandler.OnMessageAsync(message, type, cancellationToken);
            telegramBot.OnError += updateHandler.OnErrorAsync;
        }
        else
        {
            throw new Exception($"What kind of type {botWrapper.Bot.GetType()} is?");
        }
    }

}