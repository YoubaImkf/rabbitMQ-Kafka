using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqChat.Hubs;
using System.Text;

namespace RabbitMqChat.Events
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;
        private readonly bool _sendMessageToChatRoom = true; // Correctly initialized

        public RabbitMQConsumer(IHubContext<ChatHub> chatHubContext, ILogger<RabbitMQConsumer> logger)
        {
            _chatHubContext = chatHubContext;
            _logger = logger;
            _factory = new ConnectionFactory { HostName = "chat.rabbitmq" };
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _connection = _factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: "StockQuote", durable: false, exclusive: false, autoDelete: false, arguments: null);

                _logger.LogInformation("RabbitMQ connection and channel established.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error establishing RabbitMQ connection: {ex.Message}");
                throw;
            }

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => _logger.LogInformation("Background task is stopping."));

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var stockMessage = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"Message received: {stockMessage}");
                if (_sendMessageToChatRoom)
                {
                    await SendMessageToChatRoom(stockMessage);
                }
            };

            _channel.BasicConsume(queue: "StockQuote", autoAck: true, consumer: consumer);

            await Task.CompletedTask;
        }

        private async Task SendMessageToChatRoom(string message)
        {
            await _chatHubContext.Clients.All.SendAsync("ReceiveMessage", "StockBot", message);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            _connection.Close();
            _logger.LogInformation("RabbitMQ connection closed.");
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
