namespace RabbitMqChat.Events
{
    public interface IRabbitMQProducer
    {
        void SendStockMessage(string message);
    }
}
