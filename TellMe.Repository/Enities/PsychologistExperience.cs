using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Enities
{
    public class PsychologistExperience
    {
        public PsychologistExperience()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Position { get; set; } = string.Empty;   // Đổi từ Experience thành Position
        [Required]
        [MaxLength(200)]
        public string Institution { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } // Có thể null nếu đang làm
        public bool IsCurrent { get; set; } // Đánh dấu kinh nghiệm hiện tại
        [MaxLength(500)]
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        [Required]
        public Guid PsychologistId { get; set; }
        [ForeignKey("PsychologistId")]
        public virtual Psychologist Psychologist { get; set; }
    }
}
