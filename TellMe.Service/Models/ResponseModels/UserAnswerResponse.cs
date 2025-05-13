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
        public int? Score { get; set; }

        public Question Question { get; set; }
        public AnswerOption? AnswerOption { get; set; }
    }
}
