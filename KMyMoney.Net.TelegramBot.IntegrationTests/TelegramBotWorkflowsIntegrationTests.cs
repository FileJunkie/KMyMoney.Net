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
        var context = CreateContext();

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
        var context = CreateContext();

        // Act
        var state = await LoginAndGetStateAsync(context);

        // Assert
        var savedState = await context.GetSavedValueAsync($"states/{state}");
        savedState.ShouldBe("123");
    }

    [Fact]
    public async Task LoginCallback_ShouldSaveTokenAndReplayLastFailedMessage()
    {
        // Arrange
        var context = CreateContext(
            new KMyMoneyIntegrationTestContextOptions(
                OAuth2Responses:
                [
                    KMyMoneyIntegrationTestContext.CreateOAuth2Response("token-after-login")
                ]));
        await context.OnMessageAsync("/accounts");
        context.ClearMessages();
        var state = await LoginAndGetStateAsync(context);
        await SetFilePathAsync(context);

        // Act
        var callbackResult = await context.OnDropboxCallbackAsync("code-1", state);
        await Task.Delay(100);

        // Assert
        callbackResult.ShouldBeOfType<OkObjectResult>();
        (await context.GetUserSettingAsync(123, UserSettings.Token)).ShouldBe("token-after-login");
        AssertContainsMessage(context, "Id: A000001 name: Checking Account");
    }

    [Fact]
    public async Task FileWorkflow_ShouldPromptAndSaveNormalizedPath()
    {
        // Arrange
        var context = CreateContext();
        await SetTokenAsync(context);

        // Act
        await context.OnMessageAsync("/file");
        await context.OnMessageAsync("wallet.kmy");

        // Assert
        (await context.GetUserSettingAsync(123, UserSettings.Status)).ShouldBeNull();
        (await context.GetUserSettingAsync(123, UserSettings.FilePath)).ShouldBe("/wallet.kmy");
        AssertContainsMessage(context, "Choose .kmy file");
        AssertContainsMessage(context, "Got your file path, saving");
    }

    [Fact]
    public async Task AccountsWorkflow_ShouldListOnlyOpenAccounts()
    {
        // Arrange
        var context = CreateContext();
        await SetAuthorizedFileAccessAsync(context);

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
        var context = CreateContext();
        await SetAuthorizedFileAccessAsync(context);

        // Act
        await RunAddTransactionHappyPathAsync(context);

        // Assert
        (await context.GetUserSettingAsync(123, UserSettings.Status)).ShouldBe("AddTransactionEnteringFromAccount");
        AssertContainsExactMessage(context, "Saved.");
        var file = await context.LoadTestFileAsync();
        file.Root.Transactions.Values.Length.ShouldBe(1);
    }

    [Fact]
    public async Task UnknownCommandWorkflow_ShouldReturnHelp()
    {
        // Arrange
        var context = CreateContext();

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
        var context = CreateContext();
        await context.SetUserSettingAsync(123, UserSettings.Status, "AddTransactionEnteringFromAccount");
        await SetFilePathAsync(context);

        // Act
        await context.OnMessageAsync("Checking Account");

        // Assert
        AssertSingleMessageEquals(context, "Use /login to set access token");
        (await context.GetUserSettingAsync(123, UserSettings.Status)).ShouldBe("AddTransactionEnteringFromAccount");
        (await context.GetUserSettingAsync(123, UserSettings.LastFailedMessage)).ShouldNotBeNull();
    }

    [Fact]
    public async Task ErrorWorkflow_MissingFilePath_ShouldAskToSetPath()
    {
        // Arrange
        var context = CreateContext();
        await SetTokenAsync(context);

        // Act
        await context.OnMessageAsync("/accounts");

        // Assert
        AssertSingleMessageEquals(context, "Use /file to set file path");
    }

    [Fact]
    public async Task ErrorWorkflow_InvalidAccount_ShouldAbort()
    {
        // Arrange
        var context = CreateContext();
        await SetAuthorizedFileAccessAsync(context);

        // Act
        await context.OnMessageAsync("/add_transaction");
        await context.OnMessageAsync("Not existing account");

        // Assert
        AssertContainsExactMessage(context, "Wrong account, aborting");
    }

    [Fact]
    public async Task ErrorWorkflow_InvalidAmount_AndUnknownCurrency_ShouldReturnValidationErrors()
    {
        // Arrange
        var context = CreateContext();
        await SetAuthorizedFileAccessAsync(context);
        await SetAddTransactionAccountsAndPriceStatusAsync(context);

        // Act
        await context.OnMessageAsync("abc");
        await SetAddTransactionPriceStatusAsync(context);
        await context.OnMessageAsync("100 XYZ");

        // Assert
        AssertContainsExactMessage(context, "What kind of amount is that?");
        AssertContainsExactMessage(context, "What currency is that?");
    }

    [Fact]
    public async Task ErrorWorkflow_UnhandledException_ShouldSendGenericError()
    {
        // Arrange
        var context = CreateContext(
            new KMyMoneyIntegrationTestContextOptions(
                ThrowUnexpectedFileAccessError: true));
        await SetTokenAsync(context);

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
        var context = CreateContext(
            new KMyMoneyIntegrationTestContextOptions(
                OAuth2Responses:
                [
                    KMyMoneyIntegrationTestContext.CreateOAuth2Response("short-lived-token", expiresInSeconds: 1),
                    KMyMoneyIntegrationTestContext.CreateOAuth2Response("new-token")
                ]));
        var firstState = await LoginAndGetStateAsync(context);
        context.ClearMessages();

        await context.OnDropboxCallbackAsync("code-1", firstState);
        await SetFilePathAsync(context);
        context.ClearMessages();
        context.AdvanceTimeBy(TimeSpan.FromSeconds(2));

        // Act
        await context.OnMessageAsync("/accounts");
        var secondState = await LoginAndGetStateAsync(context);
        var callbackResult = await context.OnDropboxCallbackAsync("code-2", secondState);
        await Task.Delay(100);

        // Assert
        callbackResult.ShouldBeOfType<OkObjectResult>();
        AssertContainsExactMessage(context, "Use /login to set access token");
        AssertContainsMessage(context, "Id: A000001 name: Checking Account");
    }

    private static KMyMoneyIntegrationTestContext CreateContext(
        KMyMoneyIntegrationTestContextOptions? options = null) =>
        new(options);

    private static async Task<string> LoginAndGetStateAsync(KMyMoneyIntegrationTestContext context)
    {
        await context.OnMessageAsync("/login");
        var loginMessage = context.SentMessages().Last().GetRequiredText();
        loginMessage.ShouldContain("Go here:");
        loginMessage.ShouldContain("dropbox.test/oauth");
        return ExtractState(loginMessage);
    }

    private static async Task SetTokenAsync(KMyMoneyIntegrationTestContext context) =>
        await context.SetUserSettingAsync(123, UserSettings.Token, "token-1");

    private static async Task SetFilePathAsync(KMyMoneyIntegrationTestContext context) =>
        await context.SetUserSettingAsync(123, UserSettings.FilePath, "/wallet.kmy");

    private static async Task SetAuthorizedFileAccessAsync(KMyMoneyIntegrationTestContext context)
    {
        await SetTokenAsync(context);
        await SetFilePathAsync(context);
    }

    private static async Task SetAddTransactionAccountsAndPriceStatusAsync(KMyMoneyIntegrationTestContext context)
    {
        await context.SetUserSettingAsync(123, UserSettings.AccountFrom, "Checking Account");
        await context.SetUserSettingAsync(123, UserSettings.AccountTo, "Cash");
        await SetAddTransactionPriceStatusAsync(context);
    }

    private static async Task SetAddTransactionPriceStatusAsync(KMyMoneyIntegrationTestContext context) =>
        await context.SetUserSettingAsync(123, UserSettings.Status, "AddTransactionEnteringPrice");

    private static async Task RunAddTransactionHappyPathAsync(KMyMoneyIntegrationTestContext context)
    {
        await context.OnMessageAsync("/add_transaction");
        await context.OnMessageAsync("Checking Account");
        await context.OnMessageAsync("Cash");
        await context.OnMessageAsync("100");
    }

    private static void AssertSingleMessageEquals(KMyMoneyIntegrationTestContext context, string expected) =>
        context.SentMessages().Single().GetRequiredText().ShouldBe(expected);

    private static void AssertContainsExactMessage(KMyMoneyIntegrationTestContext context, string expected) =>
        context.SentMessages().Any(m => m.GetRequiredText() == expected).ShouldBeTrue();

    private static void AssertContainsMessage(KMyMoneyIntegrationTestContext context, string fragment) =>
        context.SentMessages().Any(m => m.GetRequiredText().Contains(fragment)).ShouldBeTrue();

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
