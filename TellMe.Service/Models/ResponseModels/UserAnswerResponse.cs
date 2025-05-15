using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;

namespace TellMe.Service.Models.ResponseModels
{
    public class UserAnswerResponse
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string QuestionContent { get; set; } = string.Empty;
        public Guid AnswerOptionId { get; set; }
        public string AnswerOptionContent { get; set; } = string.Empty;
        public int? Score { get; set; }
    }
}
