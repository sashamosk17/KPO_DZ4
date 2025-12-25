using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ApiGateway.Controllers;

/// <summary>
/// Gateway контроллер для проксирования запросов к OrdersService.
/// </summary>
[ApiController]
[Route("api/orders")]
public class OrdersGatewayController : ControllerBase
{
	private readonly IHttpClientFactory _httpClientFactory;

	public OrdersGatewayController(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	[HttpPost]
	public async Task<IActionResult> CreateOrder([FromBody] object request)
	{
		var client = _httpClientFactory.CreateClient("OrdersService");
		var json = JsonSerializer.Serialize(request);
		var content = new StringContent(json, Encoding.UTF8, "application/json");
		var response = await client.PostAsync("/api/orders", content);
		var responseContent = await response.Content.ReadAsStringAsync();

		return StatusCode((int)response.StatusCode, responseContent);
	}

	[HttpGet("{id:guid}")]
	public async Task<IActionResult> GetOrder(Guid id)
	{
		var client = _httpClientFactory.CreateClient("OrdersService");
		var response = await client.GetAsync($"/api/orders/{id}");
		var content = await response.Content.ReadAsStringAsync();

		return StatusCode((int)response.StatusCode, content);
	}

	[HttpGet("by-user/{userId:guid}")]
	public async Task<IActionResult> GetUserOrders(Guid userId)
	{
		var client = _httpClientFactory.CreateClient("OrdersService");
		var response = await client.GetAsync($"/api/orders/by-user/{userId}");
		var content = await response.Content.ReadAsStringAsync();

		return StatusCode((int)response.StatusCode, content);
	}
}
