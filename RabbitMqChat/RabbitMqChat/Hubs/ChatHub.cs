using Microsoft.AspNetCore.SignalR;
using RabbitMqChat.Data;
using RabbitMqChat.Data.Repositories;
using RabbitMqChat.Events;
using RabbitMqChat.Services;

namespace RabbitMqChat.Hubs
{
    public class ChatHub : Hub
    {
        public readonly ApplicationDbContext _context;
        private readonly IRabbitMQProducer _rabbitMQProducer;
        private readonly IStockService _stockService;
        private readonly IRepository<Message> _repository;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ApplicationDbContext context, IRabbitMQProducer rabbitMQProducer, IStockService stockService, IRepository<Message> repository, ILogger<ChatHub> logger)
        {
            _context = context;
            _rabbitMQProducer = rabbitMQProducer;
            _stockService = stockService;
            _repository = repository;
            _logger = logger;
        }

        public async Task SendMessage(string user, string message, string userId)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            else if (message.Contains("/stock="))
            {
                var stockCode = message.Split("=")[1];
                var stockMessage = await _stockService.GetStockInfo(stockCode);
                if (string.IsNullOrEmpty(stockMessage))
                {
                    message = "The provided stock code is invalid. Please double check it";
                    await Clients.All.SendAsync("ReceiveMessage", "StockBot", message);
                }
                else
                {
                    await Clients.All.SendAsync("ReceiveMessage", user, message);
                    _logger.LogInformation($"Calling SendStockMessage with: {stockMessage}");
                    _rabbitMQProducer.SendStockMessage(stockMessage);
                }
            }
            else
            {
                await Clients.All.SendAsync("ReceiveMessage", user, message);
                // Save messages on the database
                var messageObject = new Message
                {
                    Username = user,
                    Text = message,
                    UserId = userId
                };
                _repository.Add(messageObject);
                _repository.SaveChanges();
            }
        }
    }
}
