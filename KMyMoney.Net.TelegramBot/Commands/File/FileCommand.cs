using KMyMoney.Net.TelegramBot.Common;
using KMyMoney.Net.TelegramBot.Exceptions;
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
    AbstractMessageHandlerWithNextStep<FileEntryStatusHandler>(botClient, settingsPersistenceLayer), ICommand
{
    private readonly ITelegramBotClientWrapper _botClient = botClient;
    public string Command => "file";
    public string Description => "Setting path do the file inside Dropbox";

    protected override async Task HandleInternalAsync(Message message, CancellationToken cancellationToken)
    {
        var fileAccessor = await fileAccessService.CreateFileAccessorAsync(message, cancellationToken);

        var fileList = (await fileAccessor.ListFilesAsync()).ToList();
        if (fileList.Count == 0)
        {
            throw new WithUserMessageException("You have no .kmy files, mate");
        }

        await _botClient.Bot.SendMessageAsync(
            message.Chat.Id,
            text: "Choose .kmy file",
            replyMarkup: new[]
            {
                fileList.Select(file => new KeyboardButton(file)).ToArray()
            },
            cancellationToken:cancellationToken);
    }
}