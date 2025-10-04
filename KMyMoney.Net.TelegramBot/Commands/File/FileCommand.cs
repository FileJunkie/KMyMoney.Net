using KMyMoney.Net.TelegramBot.Common;
using KMyMoney.Net.TelegramBot.FileAccess;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace KMyMoney.Net.TelegramBot.Commands.File;

public class FileCommand(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    IFileAccessService fileAccessService,
    ITelegramBotClientWrapper botClient) :
    AbstractMessageHandlerWithNextStep<FileEntryStatusHandler>(settingsPersistenceLayer), ICommand
{
    public string Command => "file";
    public string Description => "Setting path do the file inside Dropbox";

    protected override async Task<bool> HandleInternalAsync(Message message, CancellationToken cancellationToken)
    {
        var fileAccessor = await fileAccessService.CreateFileAccessorAsync(message, cancellationToken);
        if (fileAccessor == null)
        {
            return false;
        }

        var fileList = (await fileAccessor.ListFilesAsync()).ToList();
        if (fileList.Count == 0)
        {
            await botClient.Bot.SendMessageAsync(
                message.Chat.Id,
                text: "You have no .kmy files, mate",
                cancellationToken: cancellationToken);
            return false;
        }

        await botClient.Bot.SendMessageAsync(
            message.Chat.Id,
            text: "Choose .kmy file",
            replyMarkup: new[]
            {
                fileList.Select(file => new KeyboardButton(file)).ToArray()
            },
            cancellationToken:cancellationToken);

        return true;
    }
}