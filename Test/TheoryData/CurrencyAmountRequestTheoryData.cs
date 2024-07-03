using Api.Request;

namespace Test.TheoryData;

public class CurrencyAmountRequestTheoryData : TheoryData<ICurrencyAmountRequest>
{
    public CurrencyAmountRequestTheoryData()
    {
        Add(new DepositRequest(customerId: 1, accountId: 1, amount: 10_000_000_000m));
        Add(new WithdrawalRequest(customerId: 1, accountId: 1, amount: 10_000_000_000m));
    }
}
