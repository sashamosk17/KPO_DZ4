using Microsoft.EntityFrameworkCore;
using PaymentsService.Infrastructure.Data;
using SharedLibrary.Messaging;

namespace PaymentsService.Infrastructure.BackgroundServices;

/// <summary>
/// Публикует PaymentProcessed events из Outbox в очередь payment_results.
/// </summary>
public class OutboxPublisher
{
	private readonly PaymentsDbContext _dbContext;
	private readonly IMessagePublisher _messagePublisher;
	private const string ResultsQueue = "payment_results";

	public OutboxPublisher(PaymentsDbContext dbContext, IMessagePublisher messagePublisher)
	{
		_dbContext = dbContext;
		_messagePublisher = messagePublisher;
	}

	public async Task PublishPendingEventsAsync(CancellationToken cancellationToken = default)
	{
		var events = await _dbContext.OutboxEvents
			.Where(e => !e.IsPublished && e.EventType == "PaymentProcessed")
			.OrderBy(e => e.CreatedAt)
			.Take(10)
			.ToListAsync(cancellationToken);

		foreach (var ev in events)
		{
			await _messagePublisher.PublishAsync(ResultsQueue, ev.Payload, cancellationToken);
			ev.IsPublished = true;
		}

		await _dbContext.SaveChangesAsync(cancellationToken);
	}
}
