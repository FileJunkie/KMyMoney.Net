using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core;

public class AccountRepository(KmyMoneyFile kmyMoneyFile)
{
    public IEnumerable<Account> GetAll()
    {
        return kmyMoneyFile.Accounts.Values;
    }

    public Account? GetById(string id)
    {
        return kmyMoneyFile.Accounts.Values.FirstOrDefault(a => a.Id == id);
    }

    public Account? FindByNameOrId(string nameOrId)
    {
        return kmyMoneyFile.Accounts.Values.FirstOrDefault(a => a.Id == nameOrId || a.Name == nameOrId);
    }
}
