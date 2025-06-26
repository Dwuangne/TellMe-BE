using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Enities
{
    public class PsychologicalAssessment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ExpertId { get; set; }
        [Required]
        public string ConversationTopicInformation { get; set; } 
        [Required]
        public string CurrentStatus { get; set; } 
        public string? Recommendations { get; set; }
        [Required]  
        public string ConversationOverview { get; set; }
        public DateTime AssessmentDate { get; set; }
        public DateTime EditDate { get; set; } 
        public bool IsActive { get; set; } = true;
    }
}
