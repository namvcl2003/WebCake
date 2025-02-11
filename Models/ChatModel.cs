using System;
using System.ComponentModel.DataAnnotations;
namespace ShopBaker.Models
{
    
    

    
        public class ChatModel
        {
            [Key]
            public int MessageId { get; set; }

            public int SenderId { get; set; }

            public int? ReceiverId { get; set; } // Null nếu gửi đến tất cả

            [Required]
            public string? MessageContent { get; set; }

            public DateTime SentAt { get; set; } = DateTime.Now;
        }

    public class ChatMessageDto
    {
        public string? MessageContent { get; set; }
        public int? ReceiverId { get; set; }
    }

}

