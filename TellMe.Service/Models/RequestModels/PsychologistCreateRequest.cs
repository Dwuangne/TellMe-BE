using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class PsychologistCreateRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Specialization { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        [MaxLength(200)]
        public string? AvatarUrl { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public List<PsychologistEducationRequest> Educations { get; set; } = new List<PsychologistEducationRequest>();

        public List<PsychologistExperienceRequest> Experiences { get; set; } = new List<PsychologistExperienceRequest>();

        public List<PsychologistLicenseCertificationRequest> Licenses { get; set; } = new List<PsychologistLicenseCertificationRequest>();
    }
}
