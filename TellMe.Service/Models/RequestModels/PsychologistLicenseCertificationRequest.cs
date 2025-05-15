using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class PsychologistLicenseCertificationRequest
    {
        public Guid? Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? LicenseNumber { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string IssuingAuthority { get; set; } = string.Empty;
        
        [Required]
        public DateTime IssueDate { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        
        [MaxLength(200)]
        public string? DocumentPath { get; set; }
        
        public bool IsDeleted { get; set; } = false;
    }
} 