using PaymentsService.Application.Services;

namespace PaymentsService.Infrastructure.BackgroundServices;

/// <summary>
/// Фоновая служба для периодической публикации событий из Outbox.
/// Каждые 5 секунд проверяет наличие неотправленных событий и отправляет их.
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
				var outboxPublisher = scope.ServiceProvider.GetRequiredService<IOutboxPublisher>();

				await outboxPublisher.PublishPendingEventsAsync(stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error publishing outbox events");
			}

			await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
		}

		_logger.LogInformation("OutboxPublisherWorker stopped");
	}
}
