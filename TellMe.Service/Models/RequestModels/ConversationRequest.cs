using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class ConversationRequest
    {
        [MaxLength(100, ErrorMessage = "Tên cuộc trò chuyện không được vượt quá 100 ký tự")]
        public string? Name { get; set; }

        public ICollection<MessageRequest> Messages { get; set; } = new List<MessageRequest>();

        public ICollection<ParticipantRequest> Participants { get; set; } = new List<ParticipantRequest>();
    }
}
