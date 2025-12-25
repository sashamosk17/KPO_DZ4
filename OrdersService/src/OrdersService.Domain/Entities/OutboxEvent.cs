using System.ComponentModel.DataAnnotations;
namespace OrdersService.Domain.Entities;
/// <summary>
/// Событие для шаблона Transactional Outbox. Хранится в БД в той же транзакции, что и заказ, затем отдельный воркер читает эти записи и публикует сообщения в очередь.
/// </summary>
public class OutboxEvent
{
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    [Required]
    public required string EventType { get; set; }

    [Required]
    public required string Payload { get; set; }

    public bool IsPublished { get; set; }
}
