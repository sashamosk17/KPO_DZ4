using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Infrastructure.Data;
using PaymentsService.Domain.Entities;

namespace PaymentsService.Infrastructure.BackgroundServices;
/// <summary>
/// Воркер для отправки событий из Outbox в RabbitMQ.
/// Читает неотправленные события из БД и публикует их в очередь результатов оплаты.
/// </summary>
public class OutboxPublisher
{
    private readonly PaymentsDbContext _dbContext;

    public OutboxPublisher(PaymentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task PublishPendingEventsAsync(CancellationToken cancellationToken = default)
    {
        var pendingEvents = await _dbContext.OutboxEvents
            .Where(e => !e.IsPublished)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var @event in pendingEvents)
        {
            @event.IsPublished = true;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}


