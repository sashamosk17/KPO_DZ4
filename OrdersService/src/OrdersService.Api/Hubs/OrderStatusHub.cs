using Microsoft.AspNetCore.SignalR;

namespace OrdersService.Api.Hubs;

/// <summary>
/// SignalR Hub для отправки уведомлений о смене статуса заказа клиентам.
/// Клиенты подключаются и получают real-time обновления.
/// </summary>
public class OrderStatusHub : Hub
{
	public async Task SubscribeToOrder(string orderId)
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
	}

	public async Task UnsubscribeFromOrder(string orderId)
	{
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderId);
	}
}
