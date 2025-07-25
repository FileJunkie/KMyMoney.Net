using System.Collections.Generic;
using System.Linq;
using KMyMoney.Net.Models;

namespace KMyMoney.Net.Core;

public class TransactionRepository(KmyMoneyFile kmyMoneyFile)
{
    public IEnumerable<Transaction> GetAll()
    {
        return kmyMoneyFile.Transactions;
    }

    public Transaction? GetById(string id)
    {
        return kmyMoneyFile.Transactions.FirstOrDefault(t => t.Id == id);
    }
}
