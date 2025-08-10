using KMyMoney.Net.Core.FileAccessors.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMyMoney.Net.TelegramBot.Commands.File;

public class FileCommand(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    FileEntryStatusHandler fileEntryStatusHandler,
    ITelegramBotClientWrapper botClient) :
    ICommand
{
    public string Command => "file";
    public string Description => "Setting path do the file inside Dropbox";

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var token = await settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Token,
            cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
        {
            await botClient.Bot.SendMessage(
                message.Chat.Id,
                "Log in with /login command first",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
            return;
        }

        var dropboxFileAccessor = new DropboxFileAccessor(token);
        var fileList = (await dropboxFileAccessor.ListFilesAsync()).ToList();
        if (fileList.Count == 0)
        {
            await botClient.Bot.SendMessage(
                message.Chat.Id,
                text: "You have no .kmy files, mate",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
            return;
        }

        await botClient.Bot.SendMessage(
            message.Chat.Id,
            text: "Choose .kmy file",
            replyMarkup: new[]
            {
                fileList.Select(file => new KeyboardButton(file)).ToArray()
            },
            cancellationToken:cancellationToken);

        await settingsPersistenceLayer.SetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Status,
            fileEntryStatusHandler.HandledStatus,
            cancellationToken: cancellationToken);
    }
}