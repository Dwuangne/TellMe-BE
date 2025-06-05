using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Enities
{
    public class Participant
    {
        public Guid Id { get; set; }

        public DateTime JoinedAt { get; set; }

        public Guid ConversationId { get; set; }

        public Guid UserId { get; set; }

        public virtual Conversation Conversation { get; set; } = null!;
    }
}
