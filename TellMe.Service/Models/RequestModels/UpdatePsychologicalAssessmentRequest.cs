using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class UpdatePsychologicalAssessmentRequest
    {
        [Required(ErrorMessage = "Assessment ID is required for updating.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Conversation topic information is required.")]
        public string ConversationTopicInformation { get; set; }

        [Required(ErrorMessage = "Current status is required.")]
        public string CurrentStatus { get; set; }

        public string? Recommendations { get; set; }

        [Required(ErrorMessage = "Conversation overview is required.")]
        public string ConversationOverview { get; set; }
    }
}
