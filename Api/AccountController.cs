using Api.Data;
using Api.Request;
using Api.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Api;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly AccountDbContext _context;
    public AccountController(AccountDbContext context) {
        _context = context;
    }

    [HttpPost("open")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<OpenAccountResponse> OpenAccount(OpenAccountRequest request) {
        var customer = _context.Customers.Find(request.CustomerId);

        if (customer == null) {
            return BadRequest("No customer account was found.");
        }

        var account = new Account { Customer = customer };

        _context.Accounts.Add(account);
        _context.SaveChanges();

        return CreatedAtAction(nameof(OpenAccount), new OpenAccountResponse(account.CustomerId, account.Id, true));
    }
}