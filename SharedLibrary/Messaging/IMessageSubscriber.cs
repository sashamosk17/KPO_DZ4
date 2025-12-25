namespace SharedLibrary.Messaging;
/// <summary>
/// Определяет контракт для подписки на сообщения из очереди.
/// </summary>
public interface IMessageSubscriber
{
    void Subscribe(string queue, Func<string, Task> handler);
}
