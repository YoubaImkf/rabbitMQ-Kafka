namespace RabbitMqChat.Data.DTO
{
    public class StockResult
    {
        public string? Symbol { get; set; }

        public DateTime Date { get; set; }

        public string? Time { get; set; }

        public float Open { get; set; }

        public float High { get; set; }

        public float Low { get; set; }

        public float Close { get; set; }

        public float Volume { get; set; }
    }
}
