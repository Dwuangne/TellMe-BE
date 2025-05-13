using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TellMe.Service.Models.RequestModels
{
    public class SubmitUserTestRequest
    {
        [Required]
        public Guid TestId { get; set; }
        public int TotalScore { get; set; }
        public Guid? PsychologistId { get; set; }
        public string Evaluation { get; set; } = string.Empty;

        [Required]
        public List<UserAnswerSubmission> Answers { get; set; } = new List<UserAnswerSubmission>();
    }

    public class UserAnswerSubmission
    {
        [Required]
        public Guid QuestionId { get; set; }

        [Required]
        public Guid AnswerOptionId { get; set; }
    }
} 