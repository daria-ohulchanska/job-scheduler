using JobScheduler.Shared.Configurations;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace JobScheduler.Core.Messaging
{
    public interface IMessageQueuePublisher
    {
        void SendMessage(string message);
    }

    public class RabbitMqPublisher : IMessageQueuePublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;

        public RabbitMqPublisher(IConnection connection,
            IOptions<RabbitMqSettings> settings)
        {
            _queueName = settings.Value.QueueName;
            _connection = connection;

            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                queue: _queueName, 
                durable: true, 
                exclusive: false, 
                autoDelete: false, 
                arguments: null);
        }

        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: "", 
                routingKey: _queueName, 
                basicProperties: null, 
                body: body);
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
