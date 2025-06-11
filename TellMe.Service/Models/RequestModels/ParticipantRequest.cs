using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class ParticipantRequest
    {
        public Guid? ConversationId { get; set; }

        [Required(ErrorMessage = "UserId là bắt buộc")]
        public Guid UserId { get; set; }
    }
}
