using BusinessLayer.Service;
using BusinessLayer.Service.Interface;
using Moq;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BusinessLayer.Tests
{
    [TestFixture]
    public class RabbitMQServiceTests
    {
        //TODO - Test funktionieren noch nicht

        //private Mock<IRabbitMQService> _rabbitMQServiceMock;
        //private RabbitMQService _rabbitMQService;
        //private Mock<IModel> _channelMock;
        //private Mock<IConnection> _connectionMock;
        //private Mock<ConnectionFactory> _connectionFactoryMock;

        //[SetUp]
        //public void SetUp()
        //{
        //    // Arrange
        //    _rabbitMQServiceMock = new Mock<IRabbitMQService>();
        //    _connectionMock = new Mock<IConnection>();
        //    _channelMock = new Mock<IModel>();
        //    _connectionFactoryMock = new Mock<ConnectionFactory>();

        //    _rabbitMQService = new RabbitMQService("localhost", "test_queue");
        //}

        //[Test]
        //public void SendMessage_ShouldSendMessageToQueue()
        //{
        //    // Arrange
        //    var message = new { Text = "Test Message" };
        //    var messageJson = JsonSerializer.Serialize(message);
        //    var body = Encoding.UTF8.GetBytes(messageJson);

        //    _connectionMock.Setup(conn => conn.CreateModel()).Returns(_channelMock.Object);
        //    _channelMock.Setup(channel => channel.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>()));

        //    // Act
        //    _rabbitMQService.SendMessage(message);

        //    // Assert
        //    _channelMock.Verify(c => c.BasicPublish(
        //        It.Is<string>(s => s == ""),
        //        It.Is<string>(s => s == "test_queue"),
        //        It.IsAny<IBasicProperties>(),
        //        It.Is<byte[]>(b => b.Length > 0)),
        //        Times.Once
        //    );
        //}

        //[Test]
        //public void SendMessage_ShouldHandleException_WhenConnectionFails()
        //{
        //    // Arrange
        //    var message = new { Text = "Test Message" };
        //    _connectionFactoryMock.Setup(factory => factory.CreateConnection()).Throws(new Exception("Connection failed"));

        //    // Act & Assert
        //    var ex = Assert.Throws<Exception>(() => _rabbitMQService.SendMessage(message));
        //    Assert.That(ex.Message, Is.EqualTo("Connection failed"));
        //}

        //[Test]
        //public void InitializeRabbitMQQueue_ShouldInitializeQueueSuccessfully()
        //{
        //    // Arrange
        //    _connectionMock.Setup(conn => conn.CreateModel()).Returns(_channelMock.Object);
        //    _channelMock.Setup(channel => channel.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>()));

        //    // Act
        //    _rabbitMQService.InitializeRabbitMQQueue();

        //    // Assert
        //    _channelMock.Verify(channel => channel.QueueDeclare(
        //        It.Is<string>(s => s == "ocr_queue"),
        //        It.IsAny<bool>(),
        //        It.IsAny<bool>(),
        //        It.IsAny<bool>(),
        //        It.IsAny<IDictionary<string, object>>()
        //    ), Times.Once);
        //}

        //[Test]
        //public void InitializeRabbitMQQueue_ShouldRetryOnFailure()
        //{
        //    // Arrange
        //    int retryCount = 0;
        //    _connectionMock.Setup(conn => conn.CreateModel()).Throws(new Exception("Temporary failure"));

        //    // Act & Assert
        //    var ex = Assert.Throws<Exception>(() => _rabbitMQService.InitializeRabbitMQQueue());
        //    Assert.That(ex.Message, Is.EqualTo("Temporary failure"));

        //    // Ensure retry logic is tested (maximum 10 attempts)
        //    _connectionMock.Verify(conn => conn.CreateModel(), Times.AtMost(10));
        //}
    }
}