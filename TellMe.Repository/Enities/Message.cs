using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Enities
{
    public class Message
    {
        public Guid Id { get; set; }

        public string Content { get; set; } = null!;

        public DateTime SendAt { get; set; }

        public Guid ConversationId { get; set; }

        public Guid UserId { get; set; }

        public virtual Conversation Conversation { get; set; } = null!;
    }
}
