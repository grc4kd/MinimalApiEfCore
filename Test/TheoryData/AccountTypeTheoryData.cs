using Domain.Accounts.Data;

namespace Test.TheoryData;

public class AccountTypeTheoryData : TheoryData<AccountType>
{
    public AccountTypeTheoryData()
    {
        foreach (var accountType in Enum.GetValues<AccountType>())
        {
            Add(accountType);
        }
    }
}
