using Domain.Accounts;
using Microsoft.AspNetCore.Mvc;
using Api.Responses;
using Api.Filters;
using Api.Requests;

namespace Api;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter<CurrencyActionFilterService>]
[ServiceFilter<AccountActionFilterService>]
public class AccountController(IAccountRepository repository) : ControllerBase
{
    private readonly IAccountRepository _repository = repository;

    [HttpPost("open")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Open(OpenAccountRequest request)
    {
        var response = await _repository.OpenAsync(request);

        if (response is CustomerNotFoundResponse)
        {
            return NotFound(response);
        }

        if (!response.Succeeded)
        {

            return BadRequest(response);
        }

        if (response is OpenAccountResponse openAccountResponse)
        {
            return CreatedAtAction(nameof(Open), new { id = openAccountResponse.AccountId }, openAccountResponse);
        }

        return NotFound(request);
    }

    [HttpPost("deposit")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deposit(DepositRequest request)
    {
        var response = await _repository.DepositAsync(request);

        if (response is AccountNotFoundResponse)
        {
            return NotFound(response);
        }

        if (!response.Succeeded)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(nameof(Deposit), new { id = response.AccountId }, response);
    }

    [HttpPost("withdrawal")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Withdrawal(WithdrawalRequest request)
    {
        var response = await _repository.WithdrawalAsync(request);

        if (response is AccountNotFoundResponse)
        {
            return NotFound(response);
        }

        if (!response.Succeeded)
        {

            return BadRequest(response);
        }

        return CreatedAtAction(nameof(Withdrawal), new { id = response.AccountId }, response);
    }

    [HttpPut("close")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Close(CloseAccountRequest request)
    {
        var response = await _repository.CloseAsync(request);

        if (response is AccountNotFoundResponse)
        {
            return NotFound(response);
        }

        if (!response.Succeeded)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}