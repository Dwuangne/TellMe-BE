using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TellMe.Repository.Enums;
using TellMe.Service.Models.RequestModels;

namespace TellMe.Service.Models.RequestModels
{
    public class UpdatePsychologicalTestRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public PsychologicalTestType? PsychologicalTestType { get; set; }

        public int? Duration { get; set; }

        public bool IsActive { get; set; } = true;

        public List<QuestionRequest> Questions { get; set; } = new List<QuestionRequest>();
    }
} 