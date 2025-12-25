using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

/// <summary>
/// Gateway контроллер для получения информации о платежах.
/// </summary>
[ApiController]
[Route("api/payments")]
public class PaymentsGatewayController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PaymentsGatewayController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPayment(Guid id)
    {
        var client = _httpClientFactory.CreateClient("PaymentsService");
        var response = await client.GetAsync($"/api/payments/{id}");
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }

    [HttpGet("by-order/{orderId:guid}")]
    public async Task<IActionResult> GetPaymentByOrder(Guid orderId)
    {
        var client = _httpClientFactory.CreateClient("PaymentsService");
        var response = await client.GetAsync($"/api/payments/by-order/{orderId}");
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }
}
