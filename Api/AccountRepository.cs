using Domain.Accounts;
using Domain.Accounts.Data;
using Domain.Accounts.Responses;
using Domain.Accounts.Requests;
using Infrastructure;
using Api.Responses;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace Api;

public class AccountRepository(AccountDbContext context, 
    IValidator<IOpenAccountRequest> openAccountRequestValidator,
    IValidator<IAccountTransactionRequest> accountTransactionRequestValidator) : IAccountRepository
{
    private readonly AccountDbContext _context = context;
    private readonly IValidator<IOpenAccountRequest> _openAccountRequestValidator = openAccountRequestValidator;
    private readonly IValidator<IAccountTransactionRequest> _accountTransactionRequestValidator = accountTransactionRequestValidator;

    public async Task<IOpenAccountResponse> OpenAsync(IOpenAccountRequest request)
    {
        var customer = await _context.Customers.FindAsync(request.CustomerId);

        if (customer == null)
        {
            return new CustomerNotFoundResponse(request.CustomerId);
        }

        if (request.AccountType == AccountType.Checking && !customer.HasSavingsAccount())
        {
            return new SavingsAccountDoesNotExistResponse(request.CustomerId);
        }

        var validationResult = await _openAccountRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return new OpenAccountValidationResponse(customer.Id, validationResult);
        }

        var account = customer.OpenAccount(request.AccountType, request.InitialDeposit);
        await _context.SaveChangesAsync();

        return new OpenAccountResponse(customer.Id, account.Id, _context.Entry(account).IsKeySet);
    }

    public async Task<IAccountTransactionResponse> DepositAsync(IAccountTransactionRequest request)
    {
        var validationResult = _accountTransactionRequestValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            return new AccountTransactionValidationResponse(validationResult);
        }

        var account = await _context.Accounts
            .SingleOrDefaultAsync(a =>
                a.Id == request.AccountId &&
                a.CustomerId == request.CustomerId);

        if (account == null)
        {
            return new AccountNotFoundResponse(request.AccountId);
        }

        account.MakeDeposit(request.Amount);

        await _context.SaveChangesAsync();

        return new DepositResponse(account.CustomerId, account.Id, account.Balance, _context.Entry(account).IsKeySet);
    }

    public async Task<IAccountTransactionResponse> WithdrawalAsync(IAccountTransactionRequest request)
    {
        var validationResult = _accountTransactionRequestValidator.Validate(request);

        if (!validationResult.IsValid) {
            return new AccountTransactionValidationResponse(validationResult);
        }

        var account = await _context.Accounts
            .SingleOrDefaultAsync(a =>
                a.Id == request.AccountId && a.CustomerId == request.CustomerId);

        if (account == null)
        {
            return new AccountNotFoundResponse(request.AccountId);
        }

        if (account.Balance < request.Amount)
        {
            return new InsufficientFundsResponse(account.Id);
        }

        account.MakeWithdrawal(request.Amount);
        await _context.SaveChangesAsync();

        return new WithdrawalResponse(account.CustomerId, account.Id, account.Balance, _context.Entry(account).IsKeySet);
    }

    public async Task<ICloseAccountResponse> CloseAsync(ICloseAccountRequest request)
    {
        var account = await _context.Accounts
            .SingleOrDefaultAsync(a =>
                a.Id == request.AccountId &&
                a.CustomerId == request.CustomerId);

        if (account == null)
        {
            return new AccountNotFoundResponse(request.AccountId);
        }

        if (account.Balance > 0)
        {
            return new AccountHasFundedBalanceResponse(account.Id);
        }

        if (account.AccountStatus.AccountStatusType == AccountStatusType.CLOSED)
        {
            return new AccountAlreadyClosedResponse(account.Id);
        }

        account.Close();
        await _context.SaveChangesAsync();

        return new CloseAccountResponse(account.CustomerId, account.Id, true, AccountStatusType.CLOSED);
    }
}