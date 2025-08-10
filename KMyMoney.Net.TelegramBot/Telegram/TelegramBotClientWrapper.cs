using System.Diagnostics.CodeAnalysis;
using KMyMoney.Net.TelegramBot.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace KMyMoney.Net.TelegramBot.Telegram;

// TelegramBotClient doesn't support a Stop method but just implies that cancellation token will be triggered
// Too bad it's not how ASP.NET classes are usually designed
[ExcludeFromCodeCoverage(Justification = "A thin wrapper, nothing to test")]
public sealed class TelegramBotClientWrapper : ITelegramBotClientWrapper, IAsyncDisposable
{
    private readonly CancellationTokenSource _stoppingTokenSource = new();

    public TelegramBotClient Bot { get; }

    public TelegramBotClientWrapper(IOptions<TelegramSettings> options)
    {
        Bot = new(token: options.Value.ApiToken, cancellationToken: _stoppingTokenSource.Token);
    }

    public async Task StopAsync()
    {
        await _stoppingTokenSource.CancelAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _stoppingTokenSource.Dispose();
    }
}