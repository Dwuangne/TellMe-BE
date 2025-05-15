using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Enities
{
    public class UserAnswer
    {
        public UserAnswer()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }

        public int? Score { get; set; } // Điểm cho câu trả lời này

        // Navigation properties
        [Required]
        public Guid UserTestId { get; set; }

        [Required]
        public Guid QuestionId { get; set; }

        public Guid? AnswerOptionId { get; set; } // Dành cho trắc nghiệm

        [ForeignKey("UserTestId")]
        public virtual UserTest UserTest { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }

        [ForeignKey("AnswerOptionId")]
        public virtual AnswerOption AnswerOption { get; set; }
    }
}
