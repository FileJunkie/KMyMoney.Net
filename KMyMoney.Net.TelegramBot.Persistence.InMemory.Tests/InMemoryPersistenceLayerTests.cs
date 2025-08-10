using Shouldly;

namespace KMyMoney.Net.TelegramBot.Persistence.InMemory.Tests;

public class InMemoryPersistenceLayerTests
{
    [Fact]
    public async Task GetSetUserSetting_ShouldStoreAndRetrieveValues()
    {
        // Arrange
        var persistence = new InMemoryPersistenceLayer();
        const long userId = 12345;
        const UserSettings setting = UserSettings.FilePath;
        const string value = "/path/to/file.kmy";

        // Act
        await persistence.SetUserSettingByUserIdAsync(userId, setting, value);
        var retrievedValue = await persistence.GetUserSettingByUserIdAsync(userId, setting);

        // Assert
        retrievedValue.ShouldBe(value);
    }

    [Fact]
    public async Task GetUserSetting_ShouldReturnNull_WhenNotSet()
    {
        // Arrange
        var persistence = new InMemoryPersistenceLayer();
        const long userId = 12345;
        const UserSettings setting = UserSettings.FilePath;

        // Act
        var retrievedValue = await persistence.GetUserSettingByUserIdAsync(userId, setting);

        // Assert
        retrievedValue.ShouldBeNull();
    }

    [Fact]
    public async Task SetUserSetting_WithNullValue_ShouldRemoveSetting()
    {
        // Arrange
        var persistence = new InMemoryPersistenceLayer();
        const long userId = 12345;
        const UserSettings setting = UserSettings.FilePath;
        const string initialValue = "/path/to/file.kmy";
        await persistence.SetUserSettingByUserIdAsync(userId, setting, initialValue);

        // Act
        await persistence.SetUserSettingByUserIdAsync(userId, setting, null);
        var retrievedValue = await persistence.GetUserSettingByUserIdAsync(userId, setting);

        // Assert
        retrievedValue.ShouldBeNull();
    }
}
