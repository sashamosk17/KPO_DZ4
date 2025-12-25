namespace PaymentsService.Domain.Entities;

/// <summary>
/// Счёт пользователя в платёжном сервисе.
/// Хранит баланс, с которого списываются деньги при оплате заказа.
/// </summary>
public class Account
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }
}
