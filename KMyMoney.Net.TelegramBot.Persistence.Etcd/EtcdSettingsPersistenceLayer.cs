using dotnet_etcd.interfaces;

namespace KMyMoney.Net.TelegramBot.Persistence.Etcd;

public class EtcdSettingsPersistenceLayer(IEtcdClient etcdClient) :
    ISettingsPersistenceLayer
{
    private const string Prefix = "/kmymoney.net/users";

    public async Task<string?> GetUserSettingByUserIdAsync(long userId, UserSettings setting, CancellationToken cancellationToken)
    {
        var response = await etcdClient.GetAsync(
            CreateKey(userId, setting),
            cancellationToken: cancellationToken);
        return response.Count != 0 ? response.Kvs[0].Value.ToStringUtf8().Trim() : null;
    }

    public async Task SetUserSettingByUserIdAsync(long userId, UserSettings setting, string? value, CancellationToken cancellationToken)
    {
        await etcdClient.PutAsync(CreateKey(userId, setting), value, cancellationToken: cancellationToken);
    }

    private static string CreateKey(long userId, UserSettings setting)
    {
        return $"{Prefix}/{userId}/{setting.ToString()}";
    }

}