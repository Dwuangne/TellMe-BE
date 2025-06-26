using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Enities
{
    public class PatientProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid UserId { get; set; }

        [Required]
        [MaxLength(100)] 
        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [EmailAddress] 
        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(100)] 
        public string? Occupation { get; set; } 

        [MaxLength(500)]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
