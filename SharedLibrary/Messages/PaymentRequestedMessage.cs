namespace SharedLibrary.Messages;

/// <summary>
/// Сообщение о запросе на обработку платежа
/// </summary>
public class PaymentRequestedMessage
{
	public required Guid OrderId { get; set; }
	public required Guid UserId { get; set; }
	public required decimal Amount { get; set; }
}
