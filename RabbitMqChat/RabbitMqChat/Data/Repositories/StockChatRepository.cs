namespace RabbitMqChat.Data.Repositories
{

    public class RabbitMqChatRepository : GenericRepository<Message>
    {
        public RabbitMqChatRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override IEnumerable<Message> All()
        {
            return _context.Messages
                .OrderByDescending(m => m.Date)
                .Take(50);
        }

    }

}


