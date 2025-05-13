using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Enities
{
    public class PsychologistLicenseCertification
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty; // Đổi từ LicenseCertification thành Name
        [MaxLength(50)]
        public string? LicenseNumber { get; set; }
        [Required]
        [MaxLength(200)]
        public string IssuingAuthority { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; } // Có thể null nếu không có hạn
        [MaxLength(200)]
        public string? DocumentPath { get; set; } // Đường dẫn đến file giấy phép
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        [Required]
        public Guid PsychologistId { get; set; }
        [ForeignKey("PsychologistId")]
        public virtual Psychologist Psychologist { get; set; }
    }

}
