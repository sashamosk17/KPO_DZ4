using System.ComponentModel.DataAnnotations;

namespace OrdersService.Application.Models;
/// <summary>
/// Запрос на создание нового заказа
/// </summary>
public class CreateOrderRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public required string Description { get; set; }
}
