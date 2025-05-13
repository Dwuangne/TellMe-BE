using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TellMe.Repository.Enums;

namespace TellMe.Service.Models.RequestModels
{
    public class CreatePsychologicalTestRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public PsychologicalTestType? PsychologicalTestType { get; set; }

        public int? Duration { get; set; }

        public List<QuestionRequest> Questions { get; set; } = new List<QuestionRequest>();
    }

    public class QuestionRequest
    {
        public Guid? Id { get; set; } // Null for new questions, has value for existing ones

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        [MaxLength(50)]
        public string QuestionType { get; set; } = "MultipleChoice";

        public int Order { get; set; }
        
        public int Points { get; set; } = 0;

        public List<AnswerOptionRequest> AnswerOptions { get; set; } = new List<AnswerOptionRequest>();
    }

    public class AnswerOptionRequest
    {
        public Guid? Id { get; set; } // Null for new options, has value for existing ones

        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        public int Order { get; set; }

        public int Score { get; set; }
    }
} 