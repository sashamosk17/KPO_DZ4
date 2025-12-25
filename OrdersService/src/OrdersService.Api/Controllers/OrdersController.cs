using Microsoft.AspNetCore.Mvc;
using OrdersService.Application.Models;
using OrdersService.Application.Services;

/// <summary>
/// HTTP API для работы с заказами.
/// Позволяет создавать заказ и получать информацию о заказах пользователя.
/// </summary>
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderService.CreateOrderAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetOrderAsync(id, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    [HttpGet("by-user/{userId:guid}")]
    public async Task<IActionResult> GetUserOrders(Guid userId, CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetUserOrdersAsync(userId, cancellationToken);
        return Ok(orders);
    }
}
