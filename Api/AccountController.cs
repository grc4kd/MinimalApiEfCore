using Api.Data;
using Api.Filters;
using Api.Request;
using Api.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api;

[ApiController]
[Route("api/[controller]")]
[CurrencyActionFilter]
public class AccountController(AccountDbContext context) : ControllerBase
{
    private readonly AccountDbContext _context = context;

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

        var account = new Account { Customer = customer };

        await _context.Accounts.AddAsync(account);
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

        try
        {
            account.MakeDeposit(request.Amount);
        }
        catch (ArgumentOutOfRangeException)
        {
            return BadRequest(request);
        }
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

        try
        {
            account.MakeWithdrawal(request.Amount);
        }
        catch (ArgumentOutOfRangeException)
        {
            return BadRequest(request);
        }
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Withdrawal), new WithdrawalResponse(account.CustomerId, account.Id,
            account.Balance, succeeded: true));
    }

    [HttpPut("close")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OpenAccountResponse>> Close(CloseAccountRequest request)
    {
        var customer = await _context.Customers.FindAsync(request.CustomerId);

        if (customer == null)
        {
            return NotFound(request);
        }

        var account = await _context.Accounts.FindAsync(request.AccountId);

        if (account == null)
        {
            return NotFound(request);
        }

        if (account.CustomerId != customer.Id)
        {
            return BadRequest(request);
        }

        account.Close();
        await _context.SaveChangesAsync();

        return Ok(new CloseAccountResponse(account.CustomerId, account.Id, true));
    }
}