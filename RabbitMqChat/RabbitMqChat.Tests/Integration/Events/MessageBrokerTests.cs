using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using RabbitMqChat.Events;
using RabbitMqChat.Hubs;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqChat.Tests.Integration.Events
{
    [TestClass]
    public class MessageBrokerTests
    {
        private ServiceProvider _serviceProvider;
        private IRabbitMQProducer _rabbitMQProducer;
        private RabbitMQConsumer _rabbitMQConsumer;

        [TestInitialize]
        public void SetupConsumerAndProducer()
        {
            // Set up the service collection
            var serviceCollection = new ServiceCollection();

            // Add logging
            serviceCollection.AddLogging();

            // Add SignalR hub context for ChatHub
            serviceCollection.AddSingleton<IHubContext<ChatHub>, HubContext<ChatHub>>();

            // Add RabbitMQ producer and consumer
            serviceCollection.AddScoped<IRabbitMQProducer, RabbitMQProducer>();
            serviceCollection.AddScoped<RabbitMQConsumer>();

            // Build the service provider
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Resolve the producer and consumer from the service provider
            _rabbitMQProducer = _serviceProvider.GetRequiredService<IRabbitMQProducer>();
            _rabbitMQConsumer = _serviceProvider.GetRequiredService<RabbitMQConsumer>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _serviceProvider.Dispose();
        }

        [TestMethod]
        public async Task SendAndReceiveMessageSuccessfully()
        {
            // Arrange
            var message = "This is a test message to be sent to RabbitMQ";

            // Act
            _rabbitMQProducer.SendStockMessage(message);
            var startTask = _rabbitMQConsumer.StartAsync(CancellationToken.None);
            await Task.Delay(1000);
            var stopTask = _rabbitMQConsumer.StopAsync(CancellationToken.None);
            await Task.Delay(1000);
            var mainTask = _rabbitMQConsumer.ExecuteTask;

            // Assert
            Assert.IsTrue(startTask.IsCompleted);
            Assert.IsTrue(stopTask.IsCompleted);
            Assert.IsTrue(mainTask.IsCompleted);
        }
    }

    public class HubContext<THub> : IHubContext<THub> where THub : Hub
    {
        private readonly IHubContext<THub> _hubContext;

        public HubContext(IHubContext<THub> hubContext)
        {
            _hubContext = hubContext;
        }

        public IHubClients Clients => _hubContext.Clients;
        public IGroupManager Groups => _hubContext.Groups;
    }
}
