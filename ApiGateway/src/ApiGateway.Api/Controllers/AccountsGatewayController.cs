using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsGatewayController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountsGatewayController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] object request)
    {
        var client = _httpClientFactory.CreateClient("PaymentsService");
        var response = await client.PostAsJsonAsync("/api/accounts", request);
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpPost("topup")]
    public async Task<IActionResult> TopupAccount([FromBody] object request)
    {
        var client = _httpClientFactory.CreateClient("PaymentsService");
        var response = await client.PostAsJsonAsync("/api/accounts/topup", request);
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetAccount(Guid userId)
    {
        var client = _httpClientFactory.CreateClient("PaymentsService");
        var response = await client.GetAsync($"/api/accounts/{userId}");
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }
}
