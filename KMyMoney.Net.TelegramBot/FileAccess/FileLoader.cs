using KMyMoney.Net.Core;
using KMyMoney.Net.TelegramBot.Exceptions;
using Telegram.Bot.Types;

namespace KMyMoney.Net.TelegramBot.FileAccess;

public class FileLoader(
    IFileAccessService fileAccessService) : IFileLoader
{
    public async Task<KMyMoneyFile> LoadKMyMoneyFileOrSendErrorAsync(
        Message message,
        CancellationToken cancellationToken)
    {
        var fileAccessor = await fileAccessService.CreateFileAccessorAsync(
            message,
            cancellationToken);

        var filePath = await fileAccessService.GetFilePathAsync(message, cancellationToken);

        var fileUri = new Uri($"{fileAccessor.UriPrefix}{filePath}");
        var fileLoader = new KMyMoneyLoaderBuilder()
            .WithFileAccessor(fileAccessor)
            .Build();
        var file = await fileLoader.LoadFileAsync(fileUri);
        return file;
    }
}