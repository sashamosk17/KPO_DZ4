using Microsoft.AspNetCore.Mvc;
using PaymentsService.Domain.Models;
using PaymentsService.Application.Services;

namespace PaymentsService.Api.Controllers;

/// <summary>
/// HTTP API для управления счетами пользователей:
/// создание счёта, пополнение и получение информации.
/// </summary>
[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(
        [FromBody] CreateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var account = await _accountService.CreateAccountAsync(request, cancellationToken);
        return Ok(account);
    }

    [HttpPost("topup")]
    public async Task<IActionResult> Topup(
        [FromBody] TopupAccountRequest request,
        CancellationToken cancellationToken)
    {
        var account = await _accountService.TopupAsync(request, cancellationToken);
        if (account is null)
        {
            return NotFound();
        }
        return Ok(account);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetAccount(Guid userId, CancellationToken cancellationToken)
    {
        var account = await _accountService.GetAccountAsync(userId, cancellationToken);
        if (account is null)
        {
            return NotFound();
        }
        return Ok(account);
    }


}
