using System.ComponentModel.DataAnnotations;
using OrdersService.Domain.Entities;

namespace OrdersService.Application.Models;
/// <summary>
/// Ответ с информацией о заказе
/// </summary>
public class OrderResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }

    [Required]
    public required string Description { get; set; }

    public OrderStatus Status { get; set; }
}
