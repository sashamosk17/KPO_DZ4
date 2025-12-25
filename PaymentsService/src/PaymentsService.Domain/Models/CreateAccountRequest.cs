using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Domain.Models;

/// <summary>
/// Запрос на создание нового счёта пользователя.
/// </summary>
public class CreateAccountRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal InitialBalance { get; set; }
}
