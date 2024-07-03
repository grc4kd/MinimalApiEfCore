using Api.Data;
using Api.Filters;
using Api.Request;
using Api.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter<CurrencyActionFilterService>]
[ServiceFilter<AccountActionFilterService>]
public class AccountController : ControllerBase
{
    private readonly AccountDbContext _context;

    public AccountController(AccountDbContext context)
    {
        _context = context;
    }

    [HttpPost("open")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OpenAccountResponse>> Open(OpenAccountRequest request)
    {
        var customer = await _context.Customers.FindAsync(request.CustomerId);

        if (customer == null)
        {
            return NotFound(request);
        }

        if (request.AccountType == AccountType.Checking && !customer.HasSavingsAccount()) {
            ModelState.AddModelError(nameof(request.AccountType), 
                $"{nameof(request.AccountType)} {request.AccountType} cannot be opened without creating an open {AccountType.Savings} account for customer {customer}.");
            return BadRequest(ModelState);
        }

        var account = customer.OpenAccount(request.AccountType, request.InitialDeposit);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Open), new OpenAccountResponse(account.CustomerId, account.Id, true));
    }

    [HttpPost("deposit")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DepositResponse>> Deposit(DepositRequest request)
    {
        var account = await _context.Accounts
            .SingleOrDefaultAsync(a =>
                a.Id == request.AccountId &&
                a.CustomerId == request.CustomerId);

        if (account == null)
        {
            return NotFound(request);
        }

        account.MakeDeposit(request.Amount);
            
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Deposit), new DepositResponse(account.CustomerId, account.Id,
            account.Balance, succeeded: true));
    }

    [HttpPost("withdrawal")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WithdrawalResponse>> Withdrawal(WithdrawalRequest request)
    {
        var account = await _context.Accounts
            .SingleOrDefaultAsync(a =>
                a.Id == request.AccountId &&
                a.CustomerId == request.CustomerId);

        if (account == null)
        {
            return NotFound(request);
        }

        var balanceTest = account.CheckBalanceBeforeWithdrawal(request.Amount);

        if (balanceTest.IsValid) {
            account.MakeWithdrawal(request.Amount);
            await _context.SaveChangesAsync();
        } else {
            HttpContext.Features.Set(balanceTest.Error);
            return BadRequest();
        }

        return CreatedAtAction(nameof(Withdrawal), new WithdrawalResponse(account.CustomerId, account.Id,
            account.Balance, succeeded: true));
    }

    [HttpPut("close")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OpenAccountResponse>> Close(CloseAccountRequest request)
    {
        var account = await _context.Accounts
            .SingleOrDefaultAsync(a =>
                a.Id == request.AccountId &&
                a.CustomerId == request.CustomerId);

        if (account == null)
        {
            return NotFound(request);
        }

        if (account.CustomerId != request.CustomerId)
        {
            return BadRequest(request);
        }

        account.Close();
        await _context.SaveChangesAsync();

        return Ok(new CloseAccountResponse(account.CustomerId, account.Id, true));
    }
}