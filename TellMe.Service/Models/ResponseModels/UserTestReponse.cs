using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;

namespace TellMe.Service.Models.ResponseModels
{
    public class UserTestReponse
    {
        public Guid Id { get; set; }
        public int? TotalScore { get; set; }

        [MaxLength(10000)]
        public string? Evaluation { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }

        public  PsychologicalTest PsychologicalTest { get; set; }
        public  Psychologist? Psychologist { get; set; }

        public List<UserAnswerResponse> UserAnswers { get; set; } = new List<UserAnswerResponse>();
    }
}
