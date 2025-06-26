using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class UpdatePatientProfileRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        [MaxLength(10, ErrorMessage = "Gender cannot exceed 10 characters.")]
        public string? Gender { get; set; }

        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string? Email { get; set; }

        [MaxLength(100, ErrorMessage = "Occupation cannot exceed 100 characters.")]
        public string? Occupation { get; set; }

        [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string? Address { get; set; }
    }
}
