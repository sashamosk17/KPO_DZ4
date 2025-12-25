namespace SharedLibrary.Messaging
{
	/// <summary>
	/// Определяет контракт для публикации сообщений в очередь.
	/// </summary>
	public interface IMessagePublisher
	{
		Task PublishAsync(string queue, string message, CancellationToken cancellationToken = default);
	}
}