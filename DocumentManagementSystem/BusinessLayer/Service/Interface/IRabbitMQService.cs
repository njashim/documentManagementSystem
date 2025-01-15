namespace BusinessLayer.Service.Interface
{
    public interface IRabbitMQService
    {
        void SendMessage(object message);

        void InitializeRabbitMQQueue();

        Task SendMessageAsync(string queueName, object message);
    }
}