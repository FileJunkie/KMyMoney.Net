using System.Collections.Generic;
using System.Linq;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core;

public class AccountRepository(KmyMoneyFile kmyMoneyFile)
{
    public IEnumerable<Account> GetAll()
    {
        return kmyMoneyFile.Accounts.Account;
    }

    public Account? GetById(string id)
    {
        return kmyMoneyFile.Accounts.Account.FirstOrDefault(a => a.Id == id);
    }
}
