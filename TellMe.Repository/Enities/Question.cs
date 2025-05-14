using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Enities
{
    public class Question
    {
        public Question()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            IsDeleted = false;
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string QuestionType { get; set; } = string.Empty; // Ví dụ: MultipleChoice, OpenEnded

        public int Order { get; set; }
        public int Points { get; set; } = 0;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        [Required]
        public Guid TestId { get; set; }

        [ForeignKey("TestId")]
        public virtual  PsychologicalTest Test { get; set; }
        public virtual ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
        public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }
}
