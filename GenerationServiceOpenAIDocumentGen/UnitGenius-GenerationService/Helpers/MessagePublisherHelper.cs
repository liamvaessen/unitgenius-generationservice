using System.Text;
using RabbitMQ.Client;

/// <summary>
/// Helper class for publishing messages to a RabbitMQ queue.
/// </summary>
public class MessagePublisherHelper
{
    private readonly ConnectionFactory _factory;
    private readonly string _queueName;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePublisherHelper"/> class.
    /// </summary>
    /// <param name="queueName">The name of the RabbitMQ queue.</param>
    /// <param name="hostName">The hostname of the RabbitMQ server. Default is "localhost".</param>
    /// <param name="hostUsername">The username for connecting to the RabbitMQ server. Default is "guest".</param>
    /// <param name="hostPassword">The password for connecting to the RabbitMQ server. Default is "guest".</param>
    public MessagePublisherHelper(
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
    }

    /// <summary>
    /// Publishes a message to the RabbitMQ queue.
    /// </summary>
    /// <param name="message">The message to be published.</param>
    public void PublishMessage(string message)
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: string.Empty,
            routingKey: _queueName,
            basicProperties: null,
            body: body);
    }
}