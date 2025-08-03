using System.Text;
using dotnet_etcd.interfaces;
using Etcdserverpb;
using Google.Protobuf;
using Microsoft.Extensions.Options;

namespace KMyMoney.Net.TelegramBot.Persistence.Etcd;

public class EtcdSettingsPersistenceLayer(
    IEtcdClient etcdClient,
    IOptions<EtcdSettings> etcdSettings) :
    ISettingsPersistenceLayer
{
    private string Prefix => $"{etcdSettings.Value.KeyPrefix}";

    public async Task<string?> GetSavedValueByKeyAsync(
        string key, CancellationToken cancellationToken = default)
    {
        var response = await etcdClient.GetAsync(
            $"{Prefix}/{key}",
            cancellationToken: cancellationToken);
        return response.Count != 0 ? response.Kvs[0].Value.ToStringUtf8().Trim() : null;
    }

    public Task<string?> GetUserSettingByUserIdAsync(
        long userId,
        UserSettings setting,
        CancellationToken cancellationToken = default) =>
        GetSavedValueByKeyAsync(CreateKey(userId, setting), cancellationToken);

    public Task SetUserSettingByUserIdAsync(
        long userId,
        UserSettings setting,
        string? value,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default) =>
        SetSavedValueByKeyAsync(CreateKey(userId, setting), value, expiresIn, cancellationToken);

    public async Task SetSavedValueByKeyAsync(string key, string? value, TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default)
    {
        if (value == null)
        {
            await etcdClient.DeleteAsync($"{Prefix}/{key}", cancellationToken: cancellationToken);
            return;
        }

        PutRequest putRequest;
        if (expiresIn.HasValue)
        {
            var lease = await etcdClient.LeaseGrantAsync(
                new LeaseGrantRequest
                {
                    TTL = (long)expiresIn.Value.TotalSeconds,
                }, cancellationToken: cancellationToken);
            putRequest = new PutRequest
            {
                Key = ByteString.CopyFromUtf8($"{Prefix}/{key}"),
                Value = ByteString.CopyFromUtf8(value),
                Lease = lease.ID,
            };
        }
        else
        {
            putRequest = new PutRequest
            {
                Key = ByteString.CopyFromUtf8($"{Prefix}/{key}"),
                Value = ByteString.CopyFromUtf8(value),
            };
        }

        await etcdClient.PutAsync(putRequest, cancellationToken: cancellationToken);
    }

    private static string CreateKey(long userId, UserSettings setting)
    {
        return $"users/{userId}/{setting.ToString()}";
    }

}