using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace DocumentManagementSystem.Services
{
    public class RabbitMQService
    {
        private readonly string _hostName;
        private readonly string _queueName;

        public RabbitMQService(string hostName, string queueName)
        {
            _hostName = hostName;
            _queueName = queueName;
        }

        public void SendMessage(object message)
        {
            var factory = new ConnectionFactory() { HostName = _hostName };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Stelle sicher, dass die Queue existiert
            channel.QueueDeclare(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Konvertiere die Nachricht in JSON
            var messageJson = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            // Sende die Nachricht an die RabbitMQ Queue
            channel.BasicPublish(exchange: "",
                                 routingKey: _queueName,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
