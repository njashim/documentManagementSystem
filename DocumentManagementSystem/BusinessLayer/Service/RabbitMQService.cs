using BusinessLayer.Service.Interface;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace BusinessLayer.Service
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly string _hostName;
        private readonly int _port;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _queueName;
        private readonly ILogger<RabbitMQService> _logger;

        public RabbitMQService(string hostName, int port, string userName, string password, string queueName, ILogger<RabbitMQService> logger)
        {
            _hostName = hostName;
            _port = port;
            _userName = userName;
            _password = password;
            _queueName = queueName;
            _logger = logger;
        }

        private ConnectionFactory CreateConnectionFactory()
        {
            return new ConnectionFactory
            {
                HostName = _hostName,
                Port = _port,
                UserName = _userName,
                Password = _password
            };
        }

        public void SendMessage(object message)
        {
            try
            {
                _logger.LogInformation("Attempting to send a message to queue '{QueueName}'.", _queueName);

                var factory = CreateConnectionFactory();
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(queue: _queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var messageJson = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(messageJson);

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
            _logger.LogInformation("Initializing RabbitMQ queue '{QueueName}'.", _queueName);

            var factory = CreateConnectionFactory();

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

                    channel.QueueDeclare(queue: _queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    _logger.LogInformation("RabbitMQ queue '{QueueName}' has been successfully created.", _queueName);
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

        public async Task SendMessageAsync(string queueName, object message)
        {
            try
            {
                _logger.LogInformation("Attempting to send a message to queue '{QueueName}'.", queueName);

                var factory = CreateConnectionFactory();
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var messageJson = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(messageJson);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);

                _logger.LogInformation("Message successfully sent to queue '{QueueName}'.", queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending a message to queue '{QueueName}'.", queueName);
                throw;
            }
        }

        public async Task ListenAsync(string queueName, Func<string, Task> onMessageReceived)
        {
            try
            {
                _logger.LogInformation("Starting to listen on queue '{QueueName}'.", queueName);

                var factory = CreateConnectionFactory();
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    _logger.LogInformation("Message received from queue '{QueueName}': {Message}", queueName, message);

                    try
                    {
                        await onMessageReceived(message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while processing the message from queue '{QueueName}'.", queueName);
                    }
                };

                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                await Task.Delay(-1); // Keep the listener alive
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while listening to the queue '{QueueName}'.", queueName);
                throw;
            }
        }
    }
}