using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class ChangePasswordRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The confirmation password doesn't match.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
