using RabbitMQ.Client;
using System.Text;

namespace RabbitMqChat.Events
{
    public class RabbitMQProducer : IRabbitMQProducer, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQProducer> _logger;
        private readonly string _queueName = "StockQuote";

        public RabbitMQProducer(ILogger<RabbitMQProducer> logger)
        {
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = "chat.rabbitmq"
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare the queue once during initialization
                _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                _logger.LogInformation("RabbitMQ connection and channel established.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error establishing RabbitMQ connection: {ex.Message}");
                throw;
            }
        }

        public void SendStockMessage(string message)
        {
            try
            {
                _logger.LogInformation($"Preparing to send message: {message}");
                var body = Encoding.UTF8.GetBytes(message);
                _logger.LogInformation($"Encoded message: {Convert.ToBase64String(body)}");

                _channel.BasicPublish(exchange: "", routingKey: _queueName, body: body);
                _logger.LogInformation($"Message sent to queue '{_queueName}': {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error publishing message to RabbitMQ: {ex.Message}");
            }
        }


        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ connection and channel disposed.");
        }
    }
}
