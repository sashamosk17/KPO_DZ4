using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OrdersService.Infrastructure.Data;
using OrdersService.Application.Models;
using OrdersService.Domain.Entities;

namespace OrdersService.Application.Services;


/// <summary>
/// Реализация сервиса заказов.
/// В рамках одной транзакции создаёт заказ и outbox-событие
/// для дальнейшей публикации команды оплаты в Message Queue.
/// </summary>
public class OrderService : IOrderService
{
    private readonly OrdersDbContext _dbContext;

    public OrderService(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Создаёт новый заказ и добавляет запись в OutboxEvents,
    /// чтобы затем отправить команду на оплату в PaymentsService.
    /// </summary>
    public async Task<OrderResponse> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Amount = request.Amount,
            Description = request.Description,
            Status = OrderStatus.New
        };

        var paymentCommand = new
        {
            OrderId = order.Id,
            order.UserId,
            order.Amount
        };

        var outboxEvent = new OutboxEvent
        {
            Id = Guid.NewGuid(),
            EventType = "PaymentRequested",
            Payload = JsonSerializer.Serialize(paymentCommand)
        };

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        await _dbContext.Orders.AddAsync(order, cancellationToken);
        await _dbContext.OutboxEvents.AddAsync(outboxEvent, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return MapToResponse(order);
    }

    public async Task<OrderResponse?> GetOrderAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        return order is null ? null : MapToResponse(order);
    }

    public async Task<IReadOnlyCollection<OrderResponse>> GetUserOrdersAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var orders = await _dbContext.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .ToListAsync(cancellationToken);

        return orders.Select(MapToResponse).ToArray();
    }

    /// <summary>
    /// Обновляет статус заказа, когда приходит событие об успешной или неуспешной оплате.
    /// </summary>
    public async Task UpdateOrderStatusAsync(
        Guid orderId,
        OrderStatus status,
        CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null)
        {
            return;
        }

        order.Status = status;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            UserId = order.UserId,
            Amount = order.Amount,
            Description = order.Description,
            Status = order.Status
        };
    }
}
