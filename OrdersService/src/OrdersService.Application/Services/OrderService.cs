using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrdersService.Application.Models;
using OrdersService.Domain.Entities;
using OrdersService.Infrastructure.Data;
using SharedLibrary.Messaging;

namespace OrdersService.Application.Services;
/// <summary>
/// Сервис для управления заказами и публикации событий о платежах.
/// </summary>
public class OrderService : IOrderService
{
    private readonly OrdersDbContext _db;

    public OrderService(OrdersDbContext db)
    {
        _db = db;
    }

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

        var paymentRequested = new PaymentRequestedMessage
        {
            OrderId = order.Id,
            UserId = order.UserId,
            Amount = order.Amount
        };

        var outboxEvent = new OutboxEvent
        {
            Id = Guid.NewGuid(),
            CreatedAtUtc = DateTime.UtcNow,
            EventType = "PaymentRequested",
            IsPublished = false,
            Payload = JsonSerializer.Serialize(paymentRequested)
        };

        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

        await _db.Orders.AddAsync(order, cancellationToken);
        await _db.OutboxEvents.AddAsync(outboxEvent, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return Map(order);
    }

    public async Task<OrderResponse?> GetOrderAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        return order is null ? null : Map(order);
    }

    public async Task<IReadOnlyCollection<OrderResponse>> GetUserOrdersAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var orders = await _db.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.Id)
            .ToListAsync(cancellationToken);

        return orders.Select(Map).ToArray();
    }

    public async Task UpdateOrderStatusAsync(
        Guid orderId,
        OrderStatus status,
        CancellationToken cancellationToken = default)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        if (order is null)
            return;

        order.Status = status;
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static OrderResponse Map(Order order) =>
    new()
    {
        Id = order.Id,
        UserId = order.UserId,
        Amount = order.Amount,
        Description = order.Description,
        Status = order.Status
    };
}
