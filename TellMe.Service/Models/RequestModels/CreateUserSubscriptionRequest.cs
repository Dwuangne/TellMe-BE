using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class CreateUserSubscriptionRequest
    {
        [Required]
        public int PackageId { get; set; }
    }

    public class UpdateUserSubscriptionRequest
    {
        public bool IsActive { get; set; }
    }
}
