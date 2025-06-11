using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Enities
{
    public class Conversation
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

        public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
    }
}
