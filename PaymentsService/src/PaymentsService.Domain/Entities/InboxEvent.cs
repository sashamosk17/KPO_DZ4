using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Domain.Entities;

/// <summary>
/// Событие входящей очереди (Transactional Inbox).
/// Нужен для идемпотентной обработки команд оплаты,
/// чтобы одно и то же сообщение не обработалось дважды.
/// </summary>
public class InboxEvent
{
    public Guid Id { get; set; }

    [Required]
    public required string MessageId { get; set; }

    [Required]
    public required string EventType { get; set; }

    [Required]
    public required string Payload { get; set; }

    public bool IsProcessed { get; set; }
}
