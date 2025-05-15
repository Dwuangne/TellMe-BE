using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.ResponseModels
{
    public class PsychologistLicenseCertificationResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; }
        public string IssuingAuthority { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? DocumentPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 