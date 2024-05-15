using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/// <summary>
/// Helper class for receiving messages from a RabbitMQ queue.
/// </summary>
public class MessageReceiverHelper
{
    private readonly ConnectionFactory _factory;
    private IConnection _connection;
    private IModel _channel;
    private readonly string _queueName;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageReceiverHelper"/> class.
    /// </summary>
    /// <param name="callback">The callback method to be executed when a message is received.</param>
    /// <param name="queueName">The name of the RabbitMQ queue to receive messages from.</param>
    /// <param name="hostName">The hostname of the RabbitMQ server. Default is "localhost".</param>
    /// <param name="hostUsername">The username to authenticate with the RabbitMQ server. Default is "guest".</param>
    /// <param name="hostPassword">The password to authenticate with the RabbitMQ server. Default is "guest".</param>
    public MessageReceiverHelper(
    Action<string> callback,
    string queueName,
    string hostName,
    string hostUsername,
    string hostPassword,
    int hostPort = 5672,
    string virtualHost = "/")
    {
        _queueName = queueName;
        _factory = new ConnectionFactory
        {
            HostName = hostName,
            Port = hostPort,
            UserName = hostUsername,
            Password = hostPassword,
            VirtualHost = virtualHost
        };
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            callback(message);
        };
        _channel.BasicConsume(queue: queueName,
            autoAck: true,
            consumer: consumer);
    }
}