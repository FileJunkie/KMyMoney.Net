using KMyMoney.Net.TelegramBot.Commands;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMyMoney.Net.TelegramBot;

public sealed class HostedTelegramBot(
    TelegramBotClientWrapper botWrapper,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IEnumerable<IConditionalStatusHandler> statusHandlers,
    IDefaultStatusHandler defaultStatusHandler,
    IEnumerable<ICommand> commands,
    ILogger<HostedTelegramBot> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        botWrapper.Bot.OnMessage += (message, type) => OnMessageAsync(message, type, cancellationToken);
        botWrapper.Bot.OnError += OnErrorAsync;
        await botWrapper.Bot.SetMyCommands(
            commands.Select(c => new BotCommand(c.Command, c.Description)),
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await botWrapper.StopAsync();
    }

    private async Task OnMessageAsync(Message message, UpdateType type, CancellationToken cancellationToken)
    {
        if (type != UpdateType.Message)
        {
            logger.LogTrace("UpdateType {UpdateType} ignored", type);
            return;
        }

        if (message.Chat.Type != ChatType.Private)
        {
            logger.LogTrace("MessageChatType {UpdateType} ignored", message.Chat.Type);
            return;
        }

        if (message.From == null)
        {
            logger.LogWarning("Message from is null");
            return;
        }

        try
        {
            var userStatus = await settingsPersistenceLayer.GetUserSettingByUserIdAsync(message.From.Id, UserSettings.Status, cancellationToken);
            if (!string.IsNullOrWhiteSpace(userStatus))
            {
                foreach (var statusHandler in statusHandlers)
                {
                    if (statusHandler.HandledStatus == userStatus)
                    {
                        await statusHandler.HandleAsync(message, cancellationToken);
                        return;
                    }
                }
            }

            await defaultStatusHandler.HandleAsync(message, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unhandled exception");
            await botWrapper.Bot.SendMessage(
                message.Chat.Id,
                "Sorry, mate, something went really wrong",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }
    }

    private Task OnErrorAsync(Exception exception, HandleErrorSource source)
    {
        logger.LogError(exception, "Exception in telegram bot handler, source is {Source}", source);
        return Task.CompletedTask;
    }
}