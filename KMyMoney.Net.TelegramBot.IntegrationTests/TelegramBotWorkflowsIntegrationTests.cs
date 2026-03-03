using KMyMoney.Net.TelegramBot.Persistence;
using KMyMoney.Net.TelegramBot.IntegrationTests.Extensions;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Telegram.Bot.Types.Enums;

namespace KMyMoney.Net.TelegramBot.IntegrationTests;

public class TelegramBotWorkflowsIntegrationTests
{
    [Fact]
    public async Task ReceiveUpdates_ShouldIgnore_NonMessageAndNonPrivateUpdates()
    {
        // Arrange
        var context = new KMyMoneyIntegrationTestContext();

        // Act
        await context.OnMessageAsync("/accounts", chatType: ChatType.Group);
        await context.OnMessageAsync("/accounts", updateType: UpdateType.CallbackQuery);

        // Assert
        context.SentMessages().Count.ShouldBe(0);
    }

    [Fact]
    public async Task LoginWorkflow_ShouldSendAuthorizeUrlAndSaveState()
    {
        // Arrange
        var context = new KMyMoneyIntegrationTestContext();

        // Act
        await context.OnMessageAsync("/login");

        // Assert
        var sentMessage = context.SentMessages().Single();
        sentMessage.GetRequiredText().ShouldContain("Go here:");
        sentMessage.GetRequiredText().ShouldContain("dropbox.test/oauth");
        var state = ExtractState(sentMessage.GetRequiredText());
        var savedState = await context.GetSavedValueAsync($"states/{state}");
        savedState.ShouldBe("123");
    }

    [Fact]
    public async Task LoginCallback_ShouldSaveTokenAndReplayLastFailedMessage()
    {
        // Arrange
        var context = new KMyMoneyIntegrationTestContext(
            new KMyMoneyIntegrationTestContextOptions(
                OAuth2Responses:
                [
                    KMyMoneyIntegrationTestContext.CreateOAuth2Response("token-after-login")
                ]));
        await context.OnMessageAsync("/accounts");
        context.ClearMessages();
        await context.OnMessageAsync("/login");
        var state = ExtractState(context.SentMessages().Single().GetRequiredText());
        await context.SetUserSettingAsync(123, UserSettings.FilePath, "/wallet.kmy");

        // Act
        var callbackResult = await context.OnDropboxCallbackAsync("code-1", state);
        await Task.Delay(100);

        // Assert
        callbackResult.ShouldBeOfType<OkObjectResult>();
        var token = await context.GetUserSettingAsync(123, UserSettings.Token);
        token.ShouldBe("token-after-login");
        context.SentMessages().Any(m => m.GetRequiredText().Contains("Id: A000001 name: Checking Account"))
            .ShouldBeTrue();
    }

    [Fact]
    public async Task FileWorkflow_ShouldPromptAndSaveNormalizedPath()
    {
        // Arrange
        var context = new KMyMoneyIntegrationTestContext();
        await context.SetUserSettingAsync(123, UserSettings.Token, "token-1");

        // Act
        await context.OnMessageAsync("/file");
        await context.OnMessageAsync("wallet.kmy");

        // Assert
        var status = await context.GetUserSettingAsync(123, UserSettings.Status);
        var filePath = await context.GetUserSettingAsync(123, UserSettings.FilePath);
        status.ShouldBeNull();
        filePath.ShouldBe("/wallet.kmy");
        context.SentMessages().Any(m => m.GetRequiredText().Contains("Choose .kmy file"))
            .ShouldBeTrue();
        context.SentMessages().Any(m => m.GetRequiredText().Contains("Got your file path, saving"))
            .ShouldBeTrue();
    }

    [Fact]
    public async Task AccountsWorkflow_ShouldListOnlyOpenAccounts()
    {
        // Arrange
        var context = new KMyMoneyIntegrationTestContext();
        await context.SetUserSettingAsync(123, UserSettings.Token, "token-1");
        await context.SetUserSettingAsync(123, UserSettings.FilePath, "/wallet.kmy");

        // Act
        await context.OnMessageAsync("/accounts");

        // Assert
        var joinedMessages = string.Join('\n', context.SentMessages().Select(m => m.GetRequiredText()));
        joinedMessages.ShouldContain("Id: A000001 name: Checking Account");
        joinedMessages.ShouldContain("Id: A000002 name: Cash");
        joinedMessages.ShouldNotContain("Closed account");
    }

    [Fact]
    public async Task AddTransactionWorkflow_ShouldCompleteInteractiveFlowAndPersistTransaction()
    {
        // Arrange
        var context = new KMyMoneyIntegrationTestContext();
        await context.SetUserSettingAsync(123, UserSettings.Token, "token-1");
        await context.SetUserSettingAsync(123, UserSettings.FilePath, "/wallet.kmy");

        // Act
        await context.OnMessageAsync("/add_transaction");
        await context.OnMessageAsync("Checking Account");
        await context.OnMessageAsync("Cash");
        await context.OnMessageAsync("100");

        // Assert
        var status = await context.GetUserSettingAsync(123, UserSettings.Status);
        status.ShouldBe("AddTransactionEnteringFromAccount");
        context.SentMessages().Any(m => m.Text == "Saved.").ShouldBeTrue();

        var file = await context.LoadTestFileAsync();
        file.Root.Transactions.Values.Length.ShouldBe(1);
    }

    [Fact]
    public async Task UnknownCommandWorkflow_ShouldReturnHelp()
    {
        // Arrange
        var context = new KMyMoneyIntegrationTestContext();

        // Act
        await context.OnMessageAsync("/unknown_command");

        // Assert
        var helpMessage = context.SentMessages().Single().GetRequiredText();
        helpMessage.ShouldContain("I don't exactly understand you");
        helpMessage.ShouldContain("/login");
        helpMessage.ShouldContain("/file");
        helpMessage.ShouldContain("/accounts");
        helpMessage.ShouldContain("/add_transaction");
    }

    [Fact]
    public async Task ErrorWorkflow_MissingToken_ShouldAskLoginAndKeepStatus()
    {
        // Arrange
        var context = new KMyMoneyIntegrationTestContext();
        await context.SetUserSettingAsync(123, UserSettings.Status, "AddTransactionEnteringFromAccount");
        await context.SetUserSettingAsync(123, UserSettings.FilePath, "/wallet.kmy");

        // Act
        await context.OnMessageAsync("Checking Account");

        // Assert
        context.SentMessages().Single().GetRequiredText().ShouldBe("Use /login to set access token");
        var status = await context.GetUserSettingAsync(123, UserSettings.Status);
        status.ShouldBe("AddTransactionEnteringFromAccount");
        var lastFailedMessage = await context.GetUserSettingAsync(123, UserSettings.LastFailedMessage);
        lastFailedMessage.ShouldNotBeNull();
    }

    [Fact]
    public async Task ErrorWorkflow_MissingFilePath_ShouldAskToSetPath()
    {
        // Arrange
        var context = new KMyMoneyIntegrationTestContext();
        await context.SetUserSettingAsync(123, UserSettings.Token, "token-1");

        // Act
        await context.OnMessageAsync("/accounts");

        // Assert
        context.SentMessages().Single().GetRequiredText().ShouldBe("Use /file to set file path");
    }

    [Fact]
    public async Task ErrorWorkflow_InvalidAccount_ShouldAbort()
    {
        // Arrange
        var context = new KMyMoneyIntegrationTestContext();
        await context.SetUserSettingAsync(123, UserSettings.Token, "token-1");
        await context.SetUserSettingAsync(123, UserSettings.FilePath, "/wallet.kmy");

        // Act
        await context.OnMessageAsync("/add_transaction");
        await context.OnMessageAsync("Not existing account");

        // Assert
        context.SentMessages().Any(m => m.GetRequiredText() == "Wrong account, aborting").ShouldBeTrue();
    }

    [Fact]
    public async Task ErrorWorkflow_InvalidAmount_AndUnknownCurrency_ShouldReturnValidationErrors()
    {
        // Arrange
        var context = new KMyMoneyIntegrationTestContext();
        await context.SetUserSettingAsync(123, UserSettings.Token, "token-1");
        await context.SetUserSettingAsync(123, UserSettings.FilePath, "/wallet.kmy");
        await context.SetUserSettingAsync(123, UserSettings.AccountFrom, "Checking Account");
        await context.SetUserSettingAsync(123, UserSettings.AccountTo, "Cash");
        await context.SetUserSettingAsync(123, UserSettings.Status, "AddTransactionEnteringPrice");

        // Act
        await context.OnMessageAsync("abc");
        await context.SetUserSettingAsync(123, UserSettings.Status, "AddTransactionEnteringPrice");
        await context.OnMessageAsync("100 XYZ");

        // Assert
        var sentMessages = context.SentMessages();
        sentMessages.Any(m => m.GetRequiredText() == "What kind of amount is that?").ShouldBeTrue();
        sentMessages.Any(m => m.GetRequiredText() == "What currency is that?").ShouldBeTrue();
    }

    [Fact]
    public async Task ErrorWorkflow_UnhandledException_ShouldSendGenericError()
    {
        // Arrange
        var context = new KMyMoneyIntegrationTestContext(
            new KMyMoneyIntegrationTestContextOptions(
                ThrowUnexpectedFileAccessError: true));
        await context.SetUserSettingAsync(123, UserSettings.Token, "token-1");

        // Act
        await context.OnMessageAsync("/file");

        // Assert
        var message = context.SentMessages().Single().GetRequiredText();
        message.ShouldContain("Sorry, mate, something went really wrong at");
    }

    [Fact]
    public async Task TokenExpiration_ShouldRequireRelogin_AndReplayMessageAfterNewLogin()
    {
        // Arrange
        var context = new KMyMoneyIntegrationTestContext(
            new KMyMoneyIntegrationTestContextOptions(
                OAuth2Responses:
                [
                    KMyMoneyIntegrationTestContext.CreateOAuth2Response("short-lived-token", expiresInSeconds: 1),
                    KMyMoneyIntegrationTestContext.CreateOAuth2Response("new-token")
                ]));
        await context.OnMessageAsync("/login");
        var firstState = ExtractState(context.SentMessages().Single().GetRequiredText());
        context.ClearMessages();

        await context.OnDropboxCallbackAsync("code-1", firstState);
        await context.SetUserSettingAsync(123, UserSettings.FilePath, "/wallet.kmy");
        context.ClearMessages();
        context.AdvanceTimeBy(TimeSpan.FromSeconds(2));

        // Act
        await context.OnMessageAsync("/accounts");
        await context.OnMessageAsync("/login");
        var secondState = ExtractState(
            context.SentMessages().Last(m => m.GetRequiredText().Contains("Go here:")).GetRequiredText());
        var callbackResult = await context.OnDropboxCallbackAsync("code-2", secondState);
        await Task.Delay(100);

        // Assert
        callbackResult.ShouldBeOfType<OkObjectResult>();
        context.SentMessages().Any(m => m.GetRequiredText() == "Use /login to set access token").ShouldBeTrue();
        context.SentMessages().Any(m => m.GetRequiredText().Contains("Id: A000001 name: Checking Account"))
            .ShouldBeTrue();
    }

    private static string ExtractState(string loginMessage)
    {
        const string messagePrefix = "Go here: ";
        var start = loginMessage.IndexOf(messagePrefix, StringComparison.Ordinal);
        start.ShouldBeGreaterThanOrEqualTo(0);

        var urlWithSuffix = loginMessage[(start + messagePrefix.Length)..];
        var toLogInIndex = urlWithSuffix.IndexOf(" to log in", StringComparison.Ordinal);
        toLogInIndex.ShouldBeGreaterThan(0);

        var url = urlWithSuffix[..toLogInIndex];
        var statePrefix = "state=";
        var stateIndex = url.IndexOf(statePrefix, StringComparison.Ordinal);
        stateIndex.ShouldBeGreaterThanOrEqualTo(0);
        return url[(stateIndex + statePrefix.Length)..];
    }
}
