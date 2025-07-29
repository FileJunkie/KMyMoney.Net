using KMyMoney.Net.TelegramBot.Persistence;
using Telegram.Bot;
using TgBotFramework;

namespace KMyMoney.Net.TelegramBot.Commands;

public class FileCommand(ISettingsPersistenceLayer settingsPersistenceLayer) :
    CommandBase<UpdateContext>
{
    public override async Task HandleAsync(
        UpdateContext context,
        UpdateDelegate<UpdateContext> next,
        string[] args,
        CancellationToken cancellationToken)
    {
        await context.Client.SendTextMessageAsync(
            context.Chat.Id,
            "Got your file path, saving",
            cancellationToken: cancellationToken);
        await settingsPersistenceLayer.SetFilePathByUserIdAsync(context.Sender.Id, args.Single());
    }
}