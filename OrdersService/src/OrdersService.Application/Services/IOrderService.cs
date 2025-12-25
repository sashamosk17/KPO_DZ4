using OrdersService.Application.Models;
using OrdersService.Domain.Entities;

namespace OrdersService.Application.Services;

/// <summary>
/// Сервис работы с заказами.
/// Отвечает за создание заказа и чтение его состояния,
/// инкапсулирует логику Transactional Outbox.
/// </summary>
public interface IOrderService
{
	Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);

	Task<OrderResponse?> GetOrderAsync(Guid id, CancellationToken cancellationToken = default);

	Task<IReadOnlyCollection<OrderResponse>> GetUserOrdersAsync(Guid userId, CancellationToken cancellationToken = default);

	Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status, CancellationToken cancellationToken = default);
}
