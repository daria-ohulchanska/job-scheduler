using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Microsoft.Extensions.Hosting;
using System.Text;
using Microsoft.Extensions.Options;
using JobScheduler.Shared.Configurations;

namespace JobScheduler.Core.Messaging
{
    public abstract class RabbitMqConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMqSettings _settings;

        public RabbitMqConsumer(IConnection connection, 
            IOptions<RabbitMqSettings> settings)
        {
            _settings = settings.Value;
            _connection = connection;

            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _settings.QueueName, durable: true, exclusive: false, autoDelete: false);
        }

        public abstract Task ProcessMessage(string message);

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Process the message asynchronously
                await ProcessMessage(message);

                // Acknowledge message
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: _settings.QueueName, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            base.Dispose();
        }
    }
}