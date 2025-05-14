using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Enities
{
    public class UserTest
    {
        public UserTest()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }
        [Key]
        public Guid Id { get; set; }

        public int? TotalScore { get; set; }

        [MaxLength(10000)]
        public string? Evaluation { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid TestId { get; set; }
        public Guid? PsychologistId { get; set; } // Chuyên gia được gán (nếu có)
        [ForeignKey("TestId")]
        public virtual  PsychologicalTest Test { get; set; }
        [ForeignKey("PsychologistId")]
        public virtual  Psychologist? Psychologist { get; set; }

        public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();

    }
}
