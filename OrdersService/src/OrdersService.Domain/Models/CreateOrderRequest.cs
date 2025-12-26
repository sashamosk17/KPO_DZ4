using System.ComponentModel.DataAnnotations;

namespace OrdersService.Application.Models;
/// <summary>
/// Запрос на создание нового заказа
/// </summary>
public class CreateOrderRequest
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public required string Description { get; set; }
}
