using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OrdersService.Infrastructure.Data;
using SharedLibrary.Messaging;
using SharedLibrary.Messaging;


namespace OrdersService.Api.BackgroundServices;

public class OutboxPublisher
{
    private readonly OrdersDbContext _dbContext;
    private readonly IMessagePublisher _messagePublisher;

    public OutboxPublisher(OrdersDbContext dbContext, IMessagePublisher messagePublisher)
    {
        _dbContext = dbContext;
        _messagePublisher = messagePublisher;
    }

    public async Task PublishPendingEventsAsync(CancellationToken cancellationToken = default)
    {
        var pendingEvents = await _dbContext.OutboxEvents
            .Where(e => !e.IsPublished)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var @event in pendingEvents)
        {
            await _messagePublisher.PublishAsync("payment_requests", @event.Payload, cancellationToken);
            @event.IsPublished = true;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
