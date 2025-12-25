using System.ComponentModel.DataAnnotations;

namespace OrdersService.Domain.Entities;

/// <summary>
/// Модель заказа
/// </summary>
public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }

    [Required]
    public required string Description { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.New;
}

/// <summary>
/// Статусы заказа
/// </summary>
public enum OrderStatus
{
    New = 0,
    Finished = 1,
    Cancelled = 2
}
