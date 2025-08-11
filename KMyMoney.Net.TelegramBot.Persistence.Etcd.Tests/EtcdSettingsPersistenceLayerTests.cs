using dotnet_etcd.interfaces;
using Etcdserverpb;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Mvccpb;
using NSubstitute;
using Shouldly;

namespace KMyMoney.Net.TelegramBot.Persistence.Etcd.Tests;

public class EtcdSettingsPersistenceLayerTests
{
    [Fact]
    public async Task GetUserSetting_ShouldReturnValue_WhenKeyExists()
    {
        // Arrange
        var etcdClient = Substitute.For<IEtcdClient>();
        var etcdSettings = Options.Create(new EtcdSettings
        {
            KeyPrefix = "kmymoney-test",
            ConnectionString = "http://localhost:2379",
            RootCertificate = "",
            Certificate = "",
            Key = ""
        });
        var persistence = new EtcdSettingsPersistenceLayer(etcdClient, etcdSettings);

        const long userId = 12345;
        const UserSettings setting = UserSettings.FilePath;
        const string expectedValue = "/path/to/file.kmy";
        var key = $"kmymoney-test/users/{userId}/{setting}";

        var response = new RangeResponse
        {
            Kvs = { new KeyValue { Value = ByteString.CopyFromUtf8(expectedValue) } },
            Count = 1
        };
        etcdClient.GetAsync(key).Returns(Task.FromResult(response));

        // Act
        var result = await persistence.GetUserSettingByUserIdAsync(userId, setting);

        // Assert
        result.ShouldBe(expectedValue);
    }

    [Fact]
    public async Task GetUserSetting_ShouldReturnNull_WhenKeyDoesNotExist()
    {
        // Arrange
        var etcdClient = Substitute.For<IEtcdClient>();
        var etcdSettings = Options.Create(new EtcdSettings
        {
            KeyPrefix = "kmymoney-test",
            ConnectionString = "http://localhost:2379",
            RootCertificate = "",
            Certificate = "",
            Key = ""
        });
        var persistence = new EtcdSettingsPersistenceLayer(etcdClient, etcdSettings);

        const long userId = 12345;
        const UserSettings setting = UserSettings.FilePath;
        var key = $"kmymoney-test/users/{userId}/{setting}";

        etcdClient.GetAsync(key).Returns(Task.FromResult(new RangeResponse()));

        // Act
        var result = await persistence.GetUserSettingByUserIdAsync(userId, setting);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task SetUserSetting_ShouldCallPutAsync_WithCorrectParameters()
    {
        // Arrange
        var etcdClient = Substitute.For<IEtcdClient>();
        var etcdSettings = Options.Create(new EtcdSettings
        {
            KeyPrefix = "kmymoney-test",
            ConnectionString = "http://localhost:2379",
            RootCertificate = "",
            Certificate = "",
            Key = ""
        });
        var persistence = new EtcdSettingsPersistenceLayer(etcdClient, etcdSettings);

        const long userId = 12345;
        const UserSettings setting = UserSettings.FilePath;
        const string value = "/path/to/file.kmy";
        var key = $"kmymoney-test/users/{userId}/{setting}";

        // Act
        await persistence.SetUserSettingByUserIdAsync(userId, setting, value);

        // Assert
        await etcdClient.Received(1).PutAsync(Arg.Is<PutRequest>(p =>
            p.Key.ToStringUtf8() == key &&
            p.Value.ToStringUtf8() == value
        ));
    }

    [Fact]
    public async Task SetUserSetting_WithExpiration_ShouldCallLeaseGrantAndPutAsync()
    {
        // Arrange
        var etcdClient = Substitute.For<IEtcdClient>();
        var etcdSettings = Options.Create(new EtcdSettings
        {
            KeyPrefix = "kmymoney-test",
            ConnectionString = "http://localhost:2379",
            RootCertificate = "",
            Certificate = "",
            Key = ""
        });
        var persistence = new EtcdSettingsPersistenceLayer(etcdClient, etcdSettings);

        const long userId = 12345;
        const UserSettings setting = UserSettings.FilePath;
        const string value = "/path/to/file.kmy";
        var key = $"kmymoney-test/users/{userId}/{setting}";
        var expiresIn = TimeSpan.FromMinutes(5);
        const long leaseId = 98765;

        etcdClient.LeaseGrantAsync(Arg.Any<LeaseGrantRequest>())
            .Returns(Task.FromResult(new LeaseGrantResponse { ID = leaseId }));

        // Act
        await persistence.SetUserSettingByUserIdAsync(userId, setting, value, expiresIn);

        // Assert
        await etcdClient.Received(1).LeaseGrantAsync(Arg.Is<LeaseGrantRequest>(r =>
            r.TTL == (long)expiresIn.TotalSeconds));
        await etcdClient.Received(1).PutAsync(Arg.Is<PutRequest>(p =>
            p.Key.ToStringUtf8() == key &&
            p.Value.ToStringUtf8() == value &&
            p.Lease == leaseId
        ));
    }

    [Fact]
    public async Task SetUserSetting_WithNullValue_ShouldCallDeleteAsync()
    {
        // Arrange
        var etcdClient = Substitute.For<IEtcdClient>();
        var etcdSettings = Options.Create(new EtcdSettings
        {
            KeyPrefix = "kmymoney-test",
            ConnectionString = "http://localhost:2379",
            RootCertificate = "",
            Certificate = "",
            Key = ""
        });
        var persistence = new EtcdSettingsPersistenceLayer(etcdClient, etcdSettings);

        const long userId = 12345;
        const UserSettings setting = UserSettings.FilePath;
        var key = $"kmymoney-test/users/{userId}/{setting}";

        // Act
        await persistence.SetUserSettingByUserIdAsync(userId, setting, null);

        // Assert
        await etcdClient.Received(1).DeleteAsync(key);
    }
}
