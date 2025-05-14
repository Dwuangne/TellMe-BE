using System;
using System.Collections.Generic;
using TellMe.Repository.Enums;

namespace TellMe.Service.Models.ResponseModels
{
    public class PsychologicalTestResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public PsychologicalTestType? PsychologicalTestType { get; set; }
        public int? Duration { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<QuestionResponse> Questions { get; set; } = new List<QuestionResponse>();
    }

    public class QuestionResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public int Order { get; set; }
        public int Points { get; set; }
        public List<AnswerOptionResponse> AnswerOptions { get; set; } = new List<AnswerOptionResponse>();
    }

    public class AnswerOptionResponse
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Order { get; set; }
        public int Score { get; set; }
    }
} 