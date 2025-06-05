using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.ResponseModels
{
    public class MessageResponse
    {
        public Guid Id { get; set; }

        public string Content { get; set; } = null!;

        public DateTime SendAt { get; set; }

        public Guid UserId { get; set; }
    }
}
