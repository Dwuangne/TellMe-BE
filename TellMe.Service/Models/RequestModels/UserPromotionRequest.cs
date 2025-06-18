using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class UserPromotionRequest
    {
        public Guid UserId { get; set; }
        public int PromotionId { get; set; }
        public int PromotionCount { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
