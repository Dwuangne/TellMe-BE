using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.ResponseModels
{
    public class PsychologistResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UserId { get; set; }

        public List<PsychologistEducationResponse> Educations { get; set; } = new List<PsychologistEducationResponse>();
        public List<PsychologistExperienceResponse> Experiences { get; set; } = new List<PsychologistExperienceResponse>();
        public List<PsychologistLicenseCertificationResponse> Licenses { get; set; } = new List<PsychologistLicenseCertificationResponse>();
    }
}
