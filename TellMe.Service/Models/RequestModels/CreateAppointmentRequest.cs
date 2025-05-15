using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enums;

namespace TellMe.Service.Models.RequestModels
{
    public class CreateAppointmentRequest
    {
        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Required]
        [Range(15, 180)]
        public int DurationMinutes { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Fee { get; set; }

        public string? MeetingURL { get; set; }
    }

    public class UpdateAppointmentRequest
    {
        public DateTime? AppointmentDateTime { get; set; }

        [Range(15, 180)]
        public int? DurationMinutes { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public AppointmentStatus? Status { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Fee { get; set; }

        public string? MeetingURL { get; set; }
    }
}
