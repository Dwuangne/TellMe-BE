using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;

namespace TellMe.Service.Models.ResponseModels
{
    public class UserTestResponse
    {
        public Guid Id { get; set; }
        public Guid TestId { get; set; }
        public string TestTitle { get; set; } = string.Empty;
        public int? TotalScore { get; set; }
        public string Evaluation { get; set; } = string.Empty;
        public Guid? PsychologistId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<UserAnswerResponse> UserAnswers { get; set; } = new List<UserAnswerResponse>();
    }
}
