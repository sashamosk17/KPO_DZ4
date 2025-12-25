using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Domain.Models;

/// <summary>
/// Запрос на пополнение счёта пользователя на указанную сумму.
/// </summary>
public class TopupAccountRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
}
