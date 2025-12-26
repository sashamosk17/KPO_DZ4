namespace SharedLibrary.Messaging;

/// <summary>
/// Сообщение о результате обработки платежа.
/// </summary>
public class PaymentProcessedMessage
{
    public Guid OrderId { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
