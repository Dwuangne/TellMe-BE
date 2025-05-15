using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enums;

namespace TellMe.Repository.Enities
{
    public class Appointment
    {
        public Appointment()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            Status = AppointmentStatus.Pending;
        }

        [Key]
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        [Required]
        public Guid ExpertId { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Required]
        public int DurationMinutes { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string? MeetingURL { get; set; }

        [Column(TypeName = "decimal(18,0)")]

        [Required]
        public decimal Fee { get; set; } = 0m;

        public Guid? PaymentId { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment? Payment { get; set; }

        public bool IsPaid { get; set; } = false;

        public bool IsActive { get; set; } = true;

    }
}
