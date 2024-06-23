using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RabbitMqChat.Data
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Text { get; set; }

        public DateTime Date { get; set; }

        public string UserId { get; set; }

        public virtual AppUser Sender { get; set; }

        public Message()
        {
            Date = DateTime.Now;
        }
    }
}
