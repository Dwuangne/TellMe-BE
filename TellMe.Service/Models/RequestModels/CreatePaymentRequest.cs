using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.RequestModels
{
    public class CreatePaymentRequest
    {
        public Guid UserId { get; set; }

        public Guid? AppointmentId { get; set; }

        public Guid? UserSubscriptionId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters")]
        public string PaymentMethod { get; set; } = string.Empty;

        public Guid? PaymentId { get; set; }

        // Validate that either AppointmentId or SubscriptionId is provided, but not both
        public bool IsValid()
        {
            return (AppointmentId.HasValue ^ UserSubscriptionId.HasValue); // XOR operation
        }
    }
}
