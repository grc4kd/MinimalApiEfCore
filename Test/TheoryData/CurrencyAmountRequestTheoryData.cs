using Domain.Accounts.Requests;
using Api.Requests;

namespace Test.TheoryData;

public class CurrencyAmountRequestTheoryData : TheoryData<IAccountTransactionRequest>
{
    public CurrencyAmountRequestTheoryData()
    {
        Add(new DepositRequest(customerId: 1, accountId: 1, amount: 10_000_000_000m));
        Add(new WithdrawalRequest(customerId: 1, accountId: 1, amount: 10_000_000_000m));
    }
}
