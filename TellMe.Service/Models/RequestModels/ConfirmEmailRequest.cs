using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class ConfirmEmailRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string ConfirmationLink { get; set; } = string.Empty;
    }
}
