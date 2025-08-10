using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMyMoney.Net.TelegramBot.Telegram;

public static class TelegramBotClientExtensions
{
    public static Task<Message> SendMessageAsync(
        this ITelegramBotClient botClient,
        ChatId chatId,
        string text,
        ParseMode parseMode = default,
        ReplyParameters? replyParameters = null,
        ReplyMarkup? replyMarkup = null,
        LinkPreviewOptions? linkPreviewOptions = null,
        int? messageThreadId = null,
        IEnumerable<MessageEntity>? entities = null,
        bool disableNotification = false,
        bool protectContent = false,
        string? messageEffectId = null,
        string? businessConnectionId = null,
        bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default
    )
    {
        replyMarkup ??= new ReplyKeyboardRemove();
        return botClient.SendMessage(
            chatId: chatId,
            text: text,
            parseMode: parseMode,
            replyParameters: replyParameters,
            replyMarkup: replyMarkup,
            linkPreviewOptions: linkPreviewOptions,
            messageThreadId: messageThreadId,
            entities: entities,
            disableNotification: disableNotification,
            protectContent: protectContent,
            messageEffectId: messageEffectId,
            businessConnectionId: businessConnectionId,
            allowPaidBroadcast: allowPaidBroadcast,
            cancellationToken: cancellationToken);
    }
}