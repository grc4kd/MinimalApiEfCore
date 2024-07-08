using Domain.Accounts.Requests;
using Domain.Accounts.Responses;

namespace Domain.Accounts;

public interface IAccountRepository
{
    Task<IOpenAccountResponse> OpenAsync(IOpenAccountRequest request);
    Task<IAccountTransactionResponse> DepositAsync(IAccountTransactionRequest request);
    Task<IAccountTransactionResponse> WithdrawalAsync(IAccountTransactionRequest request);
    Task<ICloseAccountResponse> CloseAsync(ICloseAccountRequest request);
}
