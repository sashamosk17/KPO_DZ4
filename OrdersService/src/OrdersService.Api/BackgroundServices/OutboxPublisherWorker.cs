namespace OrdersService.Api.BackgroundServices;

/// <summary>
/// Фоновая служба для периодической публикации событий из Outbox
/// </summary>
public class OutboxPublisherWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxPublisherWorker> _logger;

    public OutboxPublisherWorker(IServiceProvider serviceProvider, ILogger<OutboxPublisherWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxPublisherWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var publisher = scope.ServiceProvider.GetRequiredService<OutboxPublisher>();

                await publisher.PublishPendingEventsAsync(stoppingToken);
                await Task.Delay(5000, stoppingToken); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OutboxPublisherWorker");
                await Task.Delay(10000, stoppingToken);
            }
        }

        _logger.LogInformation("OutboxPublisherWorker stopped");
    }
}
