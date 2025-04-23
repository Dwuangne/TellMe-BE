using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class LoginGoogleRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
