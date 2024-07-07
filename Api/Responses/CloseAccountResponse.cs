using System.ComponentModel.DataAnnotations;
using Domain.Accounts.Data;
using Domain.Accounts.Responses;

namespace Api.Responses;

public class CloseAccountResponse(int customerId, int accountId, bool succeeded, AccountStatusType accountStatus) : ICloseAccountResponse
{
    public int AccountId { get; set; } = accountId;
    public int CustomerId { get; set; } = customerId;
    public bool Succeeded { get; set; } = succeeded;
    [EnumDataType(typeof(AccountStatusType))]
    public AccountStatusType AccountStatus { get; set; } = accountStatus;
}
