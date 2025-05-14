using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Enities
{
    public class Psychologist
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string? Specialization { get; set; }
        [MaxLength(200)]
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [MaxLength(500)]
        public string? Bio { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Required]
        public Guid UserId { get; set; }

        public virtual ICollection<PsychologistEducation> Educations { get; set; } = new List<PsychologistEducation>();
        public virtual ICollection<PsychologistExperience> Experiences { get; set; } = new List<PsychologistExperience>();
        public virtual ICollection<PsychologistLicenseCertification> Licenses { get; set; } = new List<PsychologistLicenseCertification>();
        //public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
