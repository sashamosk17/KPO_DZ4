using System.Text.Json;
using OrdersService.Application.Services;
using OrdersService.Domain.Entities;
using SharedLibrary.Messaging;
using SharedLibrary.Messages;

namespace OrdersService.Api.BackgroundServices;

/// <summary>
/// Подписчик на результаты оплаты из RabbitMQ.
/// Слушает очередь payment_results и обновляет статусы заказов.
/// </summary>
public class PaymentResultSubscriber : BackgroundService
{
    private readonly IMessageSubscriber _messageSubscriber;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PaymentResultSubscriber> _logger;

    public PaymentResultSubscriber(
        IMessageSubscriber messageSubscriber,
        IServiceProvider serviceProvider,
        ILogger<PaymentResultSubscriber> logger)
    {
        _messageSubscriber = messageSubscriber;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PaymentResultSubscriber started");

        _messageSubscriber.Subscribe("payment_results", async message =>
        {
            try
            {
                var paymentResult = JsonSerializer.Deserialize<PaymentProcessedMessage>(message);
                if (paymentResult is null)
                {
                    _logger.LogWarning("Received invalid payment result message");
                    return;
                }

                using var scope = _serviceProvider.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                var newStatus = paymentResult.Success
                    ? OrderStatus.Finished
                    : OrderStatus.Cancelled;

                await orderService.UpdateOrderStatusAsync(paymentResult.OrderId, newStatus);

                _logger.LogInformation(
                    "Order {OrderId} updated to {Status} (success = {Success})",
                    paymentResult.OrderId, newStatus, paymentResult.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment result message");
            }
        });

        return Task.CompletedTask;
    }
}
