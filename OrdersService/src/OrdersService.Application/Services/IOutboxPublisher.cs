namespace OrdersService.Application.Services;

/// <summary>
/// Воркер, который читает неотправленные события из Outbox
/// и публикует их в очередь.
/// </summary>
public interface IOutboxPublisher
{
    Task PublishPendingEventsAsync(CancellationToken cancellationToken = default);
}
