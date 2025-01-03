using BusinessLayer.Service.Interface;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BusinessLayer.Service
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly string _hostName;
        private readonly string _queueName;
        private readonly ILogger<RabbitMQService> _logger;

        public RabbitMQService(string hostName, string queueName, ILogger<RabbitMQService> logger)
        {
            _hostName = hostName;
            _queueName = queueName;
            _logger = logger;
        }

        public void SendMessage(object message)
        {
            try
            {
                _logger.LogInformation("Attempting to send a message to queue '{QueueName}'.", _queueName);

                var factory = new ConnectionFactory() { HostName = _hostName };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                // Ensure the queue exists
                channel.QueueDeclare(queue: _queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                // Convert the message to JSON
                var messageJson = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(messageJson);

                // Publish the message to the RabbitMQ queue
                channel.BasicPublish(exchange: "",
                                     routingKey: _queueName,
                                     basicProperties: null,
                                     body: body);

                _logger.LogInformation("Message successfully sent to queue '{QueueName}'.", _queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending a message to queue '{QueueName}'.", _queueName);
                throw;
            }
        }

        public void InitializeRabbitMQQueue()
        {
            _logger.LogInformation("Initializing RabbitMQ queue 'ocr_queue'.");

            var factory = new ConnectionFactory()
            {
                HostName = "dms_rabbitmq",
                Port = 5672
            };

            bool isConnected = false;
            int retryCount = 0;
            const int maxRetryCount = 10;
            const int delayMilliseconds = 5000;

            while (!isConnected && retryCount < maxRetryCount)
            {
                try
                {
                    using var connection = factory.CreateConnection();
                    using var channel = connection.CreateModel();

                    channel.QueueDeclare(queue: "ocr_queue",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    _logger.LogInformation("RabbitMQ queue 'ocr_queue' has been successfully created.");
                    isConnected = true;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    _logger.LogWarning("Failed to connect to RabbitMQ. Attempt {RetryCount}/{MaxRetryCount}: {ErrorMessage}", retryCount, maxRetryCount, ex.Message);

                    if (retryCount < maxRetryCount)
                    {
                        Thread.Sleep(delayMilliseconds);
                    }
                    else
                    {
                        _logger.LogError("Maximum number of connection attempts reached. The application will terminate.");
                        throw;
                    }
                }
            }
        }
    }
}