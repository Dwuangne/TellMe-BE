using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enums;

namespace TellMe.Service.Models.ResponseModels
{
    public class AppointmentResponse
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid ExpertId { get; set; }
        public string ExpertName { get; set; } = string.Empty;
        public DateTime AppointmentDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public string? Notes { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? MeetingURL { get; set; }
        public decimal Fee { get; set; }
        public bool IsPaid { get; set; }
        public bool IsActive { get; set; }
        public PaymentResponse? Payment { get; set; }
    }
}
