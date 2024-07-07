using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.Accounts.Data;

public class Customer
{
    public int Id { get; set; }

    [DataType(DataType.Text)]
    [StringLength(100, ErrorMessage = "There's a 100 character limit on customer name. Please shorten the name.")]
    [PersonalData]
    public required string Name { get; set; }

    public ICollection<Account> Accounts { get; } = [];

    public bool HasSavingsAccount()
    {
        return Accounts.Any(a => a.AccountType == AccountType.Savings);
    }

    public Account OpenAccount(AccountType accountType, decimal initialDeposit)
    {
        var account = new Account
        {
            CustomerId = Id,
            AccountStatus = new AccountStatus(AccountStatusType.OPEN),
            AccountType = accountType,
            Balance = initialDeposit
        };

        Accounts.Add(account);

        return account;
    }

    public override string ToString()
    {
        return string.Format("Customer# {0,8:00000000}; Name = {1}",
            Id, Name);
    }
}