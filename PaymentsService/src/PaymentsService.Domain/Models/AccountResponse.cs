namespace PaymentsService.Domain.Models;

/// <summary>
/// Информация о счёте пользователя.
/// </summary>
public class AccountResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }
}
    