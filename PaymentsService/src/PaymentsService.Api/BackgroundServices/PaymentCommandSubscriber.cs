using System.Text.Json;
using SharedLibrary.Messaging;
using SharedLibrary.Messages;
using PaymentsService.Application.Services;

namespace PaymentsService.Api.BackgroundServices;

/// <summary>
/// Подписчик на сообщения <see cref="PaymentRequestedMessage"/> из очереди RabbitMQ.
/// Обрабатывает запросы на оплату и публикует результат в очередь payment_results.
/// </summary>
public class PaymentCommandSubscriber : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageSubscriber _subscriber;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<PaymentCommandSubscriber> _logger;

    public PaymentCommandSubscriber(
        IServiceProvider serviceProvider,
        IMessageSubscriber subscriber,
        IMessagePublisher publisher,
        ILogger<PaymentCommandSubscriber> logger)
    {
        _serviceProvider = serviceProvider;
        _subscriber = subscriber;
        _publisher = publisher;
        _logger = logger;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PaymentCommandSubscriber started");

        _subscriber.Subscribe("payment_requests", async message =>
        {
            try
            {
                var paymentRequest = JsonSerializer.Deserialize<PaymentRequestedMessage>(message);
                if (paymentRequest == null)
                {
                    _logger.LogWarning("Received invalid payment request message");
                    return;
                }

                _logger.LogInformation(
                    "Received payment request for order {OrderId}",
                    paymentRequest.OrderId);

                using var scope = _serviceProvider.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IPaymentProcessor>();

                var messageId = Guid.NewGuid().ToString();

                await processor.ProcessPaymentAsync(
                    messageId,
                    paymentRequest.UserId,
                    paymentRequest.OrderId,
                    paymentRequest.Amount,
                    stoppingToken);

                var resultMessage = new PaymentProcessedMessage
                {
                    OrderId = paymentRequest.OrderId,
                    Success = true
                };

                var json = JsonSerializer.Serialize(resultMessage);
                await _publisher.PublishAsync("payment_results", json);

                _logger.LogInformation(
                    "Payment processed for order {OrderId}",
                    paymentRequest.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment message");
            }
        });

        return Task.CompletedTask;
    }
}
