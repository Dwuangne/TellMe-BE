using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Enities
{
    public class PsychologistEducation
    {
        public PsychologistEducation()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Degree { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string Institution { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [MaxLength(200)]
        public string? CertificateFile { get; set; } // Đường dẫn đến file chứng chỉ
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
