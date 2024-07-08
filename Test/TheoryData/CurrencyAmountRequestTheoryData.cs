using Domain.Accounts.Requests;
using Api.Requests;

namespace Test.TheoryData;

public class AccountTransactionRequestTheoryData : TheoryData<IAccountTransactionRequest>
{
    public AccountTransactionRequestTheoryData()
    {
        Add(new DepositRequest(customerId: 1, accountId: 1, amount: 10_000_000));
        Add(new WithdrawalRequest(customerId: 1, accountId: 1, amount: 10_000_000));
    }
}
