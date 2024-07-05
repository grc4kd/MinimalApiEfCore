using Domain.Accounts.Requests;
using Domain.Accounts.Responses;

namespace Domain.Accounts;

public interface IAccountRepository
{
    Task<IOpenAccountResponse> OpenAsync(IOpenAccountRequest request);
    Task<IDepositResponse> DepositAsync(IAccountTransactionRequest request);
    Task<IWithdrawalResponse> WithdrawalAsync(IAccountTransactionRequest request);
    Task<ICloseAccountResponse> CloseAsync(ICloseAccountRequest request);
}
