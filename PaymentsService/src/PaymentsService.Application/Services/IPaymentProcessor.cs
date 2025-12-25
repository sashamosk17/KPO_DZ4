namespace PaymentsService.Application.Services;

/// <summary>
/// Сервис, выполняющий фактическое списание денег со счёта
/// при обработке команды оплаты заказа.
/// Отвечает за идемпотентность и запись результата в Outbox.
/// </summary>
public interface IPaymentProcessor
{
    Task ProcessPaymentAsync(string messageId, Guid userId, Guid orderId, decimal amount, CancellationToken cancellationToken = default);
}
