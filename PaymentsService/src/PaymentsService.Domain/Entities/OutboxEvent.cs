using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Domain.Entities;

/// <summary>
/// Outbox-событие платёжного сервиса.
/// Используется для публикации результата оплаты в очередь.
/// </summary>
public class OutboxEvent
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public required string EventType { get; set; }

    [Required]
    public required string Payload { get; set; }

    public bool IsPublished { get; set; }
}
