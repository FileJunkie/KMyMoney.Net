namespace KMyMoney.Net.TelegramBot.Utils;

public static class UxUtils
{
    public static T[][] SplitBy<T>(this IEnumerable<T> arr, int splitBy) => arr
        .Select((x, i) => new { Index = i, Value = x })
        .GroupBy(x => x.Index / splitBy)
        .Select(g => g.Select(x => x.Value).ToArray()).ToArray();
}