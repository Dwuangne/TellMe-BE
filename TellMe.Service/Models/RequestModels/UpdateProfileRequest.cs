using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class UpdateProfileRequest
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public string Avatar { get; set; }

        public string Address { get; set; }
    }
}
