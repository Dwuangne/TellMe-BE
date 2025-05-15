using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class PsychologistExperienceRequest
    {
        public Guid? Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Position { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(200)]
        public string Institution { get; set; } = string.Empty;
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public bool IsCurrent { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public bool IsDeleted { get; set; } = false;
    }
} 