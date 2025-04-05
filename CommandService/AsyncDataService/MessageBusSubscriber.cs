
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CommandService.AsyncDataService
{
    public class MessageBusSubscriber:BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessing;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration,
           IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessing = eventProcessor;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"]),
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(
                    exchange: "trigger",
                    type: ExchangeType.Fanout);
                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(
                    queue: _queueName,
                    exchange: "trigger",
                    routingKey: "");
                _connection.ConnectionShutdown += (sender, e)=>
                {
                    Console.WriteLine("--> Connection shutdown");
                };
                Console.WriteLine("--> Listening to the message bus...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the message bus: {ex.Message}");
            }
        }
        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (MouduleHandle, ea) =>
            {
                Console.WriteLine("--> Event received");
                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                _eventProcessing.ProcessEvent(notificationMessage);
            };
            _channel.BasicConsume(
                queue: _queueName,
                autoAck: true,
                consumer: consumer);
            return Task.CompletedTask;
        }
    }
}
