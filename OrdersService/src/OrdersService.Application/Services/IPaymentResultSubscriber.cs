namespace OrdersService.Application.Services;

/// <summary>
/// Сервис, подписывающийся на результаты оплаты из очереди и обновляющий статусы заказов.
/// </summary>
public interface IPaymentResultSubscriber
{
    void Start();
}
