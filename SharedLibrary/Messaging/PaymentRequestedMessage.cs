namespace SharedLibrary.Messaging;

/// <summary>
/// Сообщение о запросе на обработку платежа.
/// </summary>
public class PaymentRequestedMessage
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
}
