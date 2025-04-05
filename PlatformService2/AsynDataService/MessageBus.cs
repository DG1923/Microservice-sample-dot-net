using PlatformService2.DTOs;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PlatformService2.AsynDataService
{
    //Can use Transmiss easier configuration
    public class MessageBus : IMessageBus
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;//use version 6.2.2 it works and latest version 7.0.2 does not work

        public MessageBus(IConfiguration configuration)
        {
            _configuration = configuration;
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
                //create func to listen to the event
                _connection.ConnectionShutdown += (sender, e) =>
                {
                    Console.WriteLine("--> Connection shutdown");
                };
                Console.WriteLine("--> Connected to the message bus");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the message bus: {ex.Message}");
            }
        }
        public void PublishNewPlatform(PlatformPublishDTO platformPublishDto)
        {
            var message = JsonSerializer.Serialize(platformPublishDto);
            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ connection open, sending message...");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection is closed, not sending");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: "trigger",
                routingKey:"",
                basicProperties: null,
                body: body);
            Console.WriteLine($"--> We have sent {message}");
        }
        public void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
    }
}
