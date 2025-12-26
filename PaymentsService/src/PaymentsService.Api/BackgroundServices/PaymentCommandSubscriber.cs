using System.Text.Json;
using PaymentsService.Application.Services;
using SharedLibrary.Messaging;

namespace PaymentsService.Api.BackgroundServices;
/// <summary>
/// Слушает очередь payments.commands и передаёт сообщения в PaymentProcessor.
/// </summary>
public class PaymentCommandSubscriber : BackgroundService
{
    private readonly IMessageSubscriber _messageSubscriber;
    private readonly IServiceProvider _serviceProvider;

    public PaymentCommandSubscriber(IMessageSubscriber messageSubscriber, IServiceProvider serviceProvider)
    {
        _messageSubscriber = messageSubscriber;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageSubscriber.Subscribe("payment_requests", async raw =>
        {
            var message = JsonSerializer.Deserialize<PaymentRequestedMessage>(raw);
            if (message is null)
            {
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IPaymentProcessor>();

            await processor.ProcessPaymentAsync(
                message.OrderId.ToString(),
                message.UserId,
                message.OrderId,
                message.Amount,
                stoppingToken);
        });

        return Task.CompletedTask;
    }
}
