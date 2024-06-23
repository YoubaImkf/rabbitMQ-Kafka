using Microsoft.AspNetCore.Identity;

namespace RabbitMqChat.Data
{
    public class AppUser : IdentityUser
    {
        public virtual ICollection<Message> Messages { get; set; }

        public AppUser()
        {
            Messages = new HashSet<Message>();
        }

    }
}
