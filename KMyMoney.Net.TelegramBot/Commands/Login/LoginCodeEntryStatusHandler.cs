using Dropbox.Api;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Settings;
using KMyMoney.Net.TelegramBot.StatusHandlers;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMyMoney.Net.TelegramBot.Commands.Login;

public class LoginCodeEntryStatusHandler(
    TelegramBotClientWrapper botWrapper,
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IOptions<DropboxSettings> dropboxSettings) : IConditionalStatusHandler
{
    public string HandledStatus => "EnteringLoginCode";

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        await botWrapper.Bot.DeleteMessage(message.Chat.Id, message.Id, cancellationToken);
        await botWrapper.Bot.SendMessage(
            message.Chat.Id,
            "Got your code, getting and saving token",
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
        await settingsPersistenceLayer.SetUserSettingByUserIdAsync(message.From!.Id, UserSettings.Status, null, cancellationToken);
        var token = await DropboxOAuth2Helper.ProcessCodeFlowAsync(
            message.Text,
            dropboxSettings.Value.ApiKey,
            dropboxSettings.Value.ApiSecret);

        if (token == null)
        {
            await botWrapper.Bot.SendMessage(
                message.Chat.Id,
                "Something went wrong with getting token",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
            return;
        }

        await settingsPersistenceLayer.SetUserSettingByUserIdAsync(message.From!.Id, UserSettings.Token, token.AccessToken, cancellationToken);
    }
}