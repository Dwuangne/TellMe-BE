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
    public class Payment
    {
        public Payment()
        {
            Id = Guid.NewGuid();
            PaymentDate = DateTime.Now;
            Status = PaymentStatus.Pending;
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public Guid? AppointmentId { get; set; }

        public Guid? SubscriptionId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; }

        [Required]
        public PaymentStatus Status { get; set; } 

        public string? TransactionId { get; set; }

        public DateTime PaymentDate { get; set; }

        public virtual Appointment? Appointment { get; set; }

        public virtual UserSubscription? Subscription { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
