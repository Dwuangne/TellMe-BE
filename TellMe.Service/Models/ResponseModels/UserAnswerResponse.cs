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
        public string QuestionText { get; set; } // Chỉ lấy nội dung câu hỏi
        public Guid AnswerOptionId { get; set; }
        public string AnswerOptionText { get; set; } // Chỉ lấy nội dung tùy chọn
        public int? Score { get; set; }
    }
}
