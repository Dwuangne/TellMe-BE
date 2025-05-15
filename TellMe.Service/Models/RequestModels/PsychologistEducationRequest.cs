using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class PsychologistEducationRequest
    {
        public Guid? Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Degree { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(200)]
        public string Institution { get; set; } = string.Empty;
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [MaxLength(200)]
        public string? CertificateFile { get; set; }
        
        public bool IsDeleted { get; set; } = false;
    }
} 