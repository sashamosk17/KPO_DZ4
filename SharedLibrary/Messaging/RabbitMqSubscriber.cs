using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SharedLibrary.Messaging;
/// <summary>
/// Реализация подписчика сообщений, использующая RabbitMQ для получения сообщений из очередей.
/// Устанавливает подключение и канал к брокеру сообщений и передает полученные сообщения в заданный обработчик.
/// </summary>
public class RabbitMqSubscriber : IMessageSubscriber
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqSubscriber(string connectionString)
    {
        var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Subscribe(string queue, Func<string, Task> handler)
    {
        _channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            await handler(message);
        };

        _channel.BasicConsume(queue, autoAck: true, consumer);
    }
}
