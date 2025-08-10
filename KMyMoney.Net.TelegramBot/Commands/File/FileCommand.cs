using KMyMoney.Net.Core.FileAccessors.Dropbox;
using KMyMoney.Net.TelegramBot.Common;
using KMyMoney.Net.TelegramBot.Dropbox;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMyMoney.Net.TelegramBot.Commands.File;

public class FileCommand(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    FileEntryStatusHandler fileEntryStatusHandler,
    IFileAccessorFactory fileAccessorFactory,
    ITelegramBotClientWrapper botClient) :
    AbstractMessageHandlerWithNextStep(settingsPersistenceLayer, fileEntryStatusHandler), ICommand
{
    private readonly ISettingsPersistenceLayer _settingsPersistenceLayer = settingsPersistenceLayer;
    public string Command => "file";
    public string Description => "Setting path do the file inside Dropbox";

    protected override async Task HandleInternalAsync(Message message, CancellationToken cancellationToken)
    {
        var token = await _settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Token,
            cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
        {
            await botClient.Bot.SendMessageAsync(
                message.Chat.Id,
                "Log in with /login command first",
                cancellationToken: cancellationToken);
            return;
        }

        var dropboxFileAccessor = fileAccessorFactory.CreateFileAccessor(token);
        var fileList = (await dropboxFileAccessor.ListFilesAsync()).ToList();
        if (fileList.Count == 0)
        {
            await botClient.Bot.SendMessageAsync(
                message.Chat.Id,
                text: "You have no .kmy files, mate",
                cancellationToken: cancellationToken);
            return;
        }

        await botClient.Bot.SendMessageAsync(
            message.Chat.Id,
            text: "Choose .kmy file",
            replyMarkup: new[]
            {
                fileList.Select(file => new KeyboardButton(file)).ToArray()
            },
            cancellationToken:cancellationToken);
    }
}