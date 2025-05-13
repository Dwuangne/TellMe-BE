using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enums;

namespace TellMe.Repository.Enities
{
    public class PsychologicalTest
    {
        public PsychologicalTest()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            IsActive = true;
        }
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty; // Tên bài test

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public PsychologicalTestType? PsychologicalTestType { get; set; }

        public int? Duration { get; set; } // Thời gian (phút)

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
        public virtual ICollection<UserTest> UserTests { get; set; } = new List<UserTest>();
    }
}
