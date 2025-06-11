using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.ResponseModels
{
    public class ParticipantResponse
    {
        public Guid Id { get; set; }

        public DateTime JoinedAt { get; set; }

        public Guid ConversationId { get; set; }

        public Guid UserId { get; set; }

        public string FullName { get; set; } = string.Empty;
    }
}
