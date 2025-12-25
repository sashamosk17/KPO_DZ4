namespace PaymentsService.Application.Services;

/// <summary>
/// Сервис, подписывающийся на команды оплаты из очереди
/// и вызывающий PaymentProcessor для их обработки.
/// </summary>
public interface IPaymentCommandSubscriber
{
    void Start();
}
