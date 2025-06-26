using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class CreatePsychologicalAssessmentRequest
    {
        [Required(ErrorMessage = "User ID is required for the assessment.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Expert ID is required for the assessment.")]
        public Guid ExpertId { get; set; }

        [Required(ErrorMessage = "Conversation topic information is required.")]
        public string ConversationTopicInformation { get; set; }

        [Required(ErrorMessage = "Current status is required.")]
        public string CurrentStatus { get; set; }

        public string? Recommendations { get; set; }

        [Required(ErrorMessage = "Conversation overview is required.")]
        public string ConversationOverview { get; set; }
    }
}
