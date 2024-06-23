using RabbitMqChat.Events;

namespace RabbitMqChat.Tests.Integration.Events
{
    [TestClass]
    public class MessageBrokerTests
    {
        public IRabbitMQProducer? rabbitMQProducer;
        public RabbitMQConsumer? rabbitMQConsumer;

        [TestInitialize]
        public void SetupConsumerAndProducer()
        {
            rabbitMQProducer = new RabbitMQProducer();
            rabbitMQConsumer = new RabbitMQConsumer();
        }

        [TestMethod]
        public async Task SendAndReceiveMessageSuccesfully()
        {
            //Arrange
            var message = "This is a test message to be sent to RabbitMQ";

            //Act
            rabbitMQProducer.SendStockMessage(message);
            var startTask = rabbitMQConsumer.StartAsync(CancellationToken.None);
            await Task.Delay(1000);
            var stopTask = rabbitMQConsumer.StopAsync(CancellationToken.None);
            await Task.Delay(1000);
            var mainTask = rabbitMQConsumer.ExecuteTask;

            //Assert
            Assert.IsTrue(startTask.IsCompleted);
            Assert.IsTrue(stopTask.IsCompleted);
            Assert.IsTrue(mainTask.IsCompleted);
        }
    }
}
