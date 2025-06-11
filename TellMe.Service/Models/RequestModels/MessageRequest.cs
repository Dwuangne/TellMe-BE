using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class MessageRequest
    {
        [Required(ErrorMessage = "Nội dung tin nhắn là bắt buộc")]
        [MaxLength(1000, ErrorMessage = "Nội dung tin nhắn không được vượt quá 1000 ký tự")]
        public string Content { get; set; } = null!;

        public Guid? ConversationId { get; set; }

        [Required(ErrorMessage = "UserId là bắt buộc")]
        public Guid UserId { get; set; }
    }
}
