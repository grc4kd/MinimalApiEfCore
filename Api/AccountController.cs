using Api.Data;
using Api.Filters;
using Api.Request;
using Api.Responses;
using Microsoft.AspNetCore.Mvc;

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
    public ActionResult<OpenAccountResponse> Open(OpenAccountRequest request)
    {
        var customer = _context.Customers.Find(request.CustomerId);

        if (customer == null)
        {
            return NotFound(request);
        }

        var account = new Account { Customer = customer };

        _context.Accounts.Add(account);
        _context.SaveChanges();

        return CreatedAtAction(nameof(Open), new OpenAccountResponse(account.CustomerId, account.Id, true));
    }

    [HttpPost("deposit")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<DepositResponse> Deposit(DepositRequest request)
    {
        var account = _context.Accounts.Where(a =>
            a.Id == request.AccountId &&
            a.CustomerId == request.CustomerId)
            .SingleOrDefault();

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
        _context.SaveChanges();

        return CreatedAtAction(nameof(Deposit), new DepositResponse(account.CustomerId, account.Id,
            account.Balance, succeeded: true));
    }

    [HttpPost("withdrawal")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<WithdrawalResponse> Withdrawal(WithdrawalRequest request)
    {
        var account = _context.Accounts.Where(a =>
            a.Id == request.AccountId &&
            a.CustomerId == request.CustomerId)
            .SingleOrDefault();
        
        if (account == null) {
            return NotFound(request);
        }

        try {
            account.MakeWithdrawal((decimal)request.Amount);
        } catch (ArgumentOutOfRangeException) {
            return BadRequest(request);
        }
        _context.SaveChanges();

        return CreatedAtAction(nameof(Withdrawal), new WithdrawalResponse(account.CustomerId, account.Id, 
            account.Balance, succeeded: true));
    }

    [HttpPut("close")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<OpenAccountResponse> Close(CloseAccountRequest request)
    {
        var customer = _context.Customers.Find(request.CustomerId);

        if (customer == null)
        {
            return NotFound(request);
        }

        var account = _context.Accounts.Find(request.AccountId);

        if (account == null)
        {
            return NotFound(request);
        }

        if (account.CustomerId != customer.Id)
        {
            return BadRequest(request);
        }

        account.Close();
        _context.SaveChanges();

        return Ok(new CloseAccountResponse(account.CustomerId, account.Id, true));
    }
}