using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.ResponseModels
{
    public class ConversationResponse
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<MessageResponse> Messages { get; set; } = new List<MessageResponse>();

        public List<ParticipantResponse> Participants { get; set; } = new List<ParticipantResponse>();
    }
}
