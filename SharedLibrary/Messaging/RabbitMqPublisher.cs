using System.Text;
using RabbitMQ.Client;

namespace SharedLibrary.Messaging;
/// <summary>
/// Реализация издателя сообщений, использующая RabbitMQ для отправки сообщений в указанные очереди.
/// Создает подключение и канал к брокеру сообщений и публикует сообщения в устойчивые очереди.
/// </summary>
public class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqPublisher(string connectionString)
    {
        var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task PublishAsync(string queue, string message, CancellationToken cancellationToken = default)
    {
        _channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish("", queue, null, body);
        return Task.CompletedTask;
    }
}
