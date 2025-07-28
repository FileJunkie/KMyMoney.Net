using KMyMoney.Net.Core.FileAccessors;

namespace KMyMoney.Net.Core;

public class KMyMoneyLoaderBuilder
{
    private readonly IList<IFileAccessor> _fileAccessors = [];

    public KMyMoneyLoaderBuilder WithFileAccessor(IFileAccessor fileAccessor)
    {
        _fileAccessors.Add(fileAccessor);
        return this;
    }

    public KMyMoneyLoader Build() =>
        new(_fileAccessors);
}