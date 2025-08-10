using KMyMoney.Net.Core;
using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.Telegram;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.Dropbox;

public class FileLoader(
    ISettingsPersistenceLayer settingsPersistenceLayer,
    ITelegramBotClientWrapper botClientWrapper,
    IFileAccessorFactory fileAccessorFactory) : IFileLoader
{
    public async Task<KMyMoneyFile?> LoadKMyMoneyFileOrSendErrorAsync(
        Message message,
        CancellationToken cancellationToken)
    {
        var token = await settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.Token,
            cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
        {
            await botClientWrapper.Bot.SendMessageAsync(
                message.Chat.Id,
                "Use /login to set access token",
                cancellationToken: cancellationToken);
            return null;
        }

        var filePath = await settingsPersistenceLayer.GetUserSettingByUserIdAsync(
            message.From!.Id,
            UserSettings.FilePath,
            cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(filePath))
        {
            await botClientWrapper.Bot.SendMessageAsync(
                message.Chat.Id,
                "Use /file to set file path",
                cancellationToken: cancellationToken);
            return null;
        }

        var fileAccessor = fileAccessorFactory.CreateFileAccessor(token);
        var fileUri = new Uri($"dropbox://{filePath}");
        var fileLoader = new KMyMoneyLoaderBuilder()
            .WithFileAccessor(fileAccessor)
            .Build();
        var file = await fileLoader.LoadFileAsync(fileUri);
        return file;
    }
}