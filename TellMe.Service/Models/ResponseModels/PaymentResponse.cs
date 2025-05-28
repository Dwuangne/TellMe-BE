using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enums;

namespace TellMe.Service.Models.ResponseModels
{
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? AppointmentId { get; set; }
        public Guid? UserSubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public long? OrderCode { get; set; }
        public string? TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public bool IsActive { get; set; }

        public AppointmentBasicInfo? AppointmentInfo { get; set; }

        public SubscriptionBasicInfo? SubscriptionInfo { get; set; }

        public ProfileResponse? User { get; set; }
    }

    public class AppointmentBasicInfo
    {
        public Guid Id { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public decimal Fee { get; set; }
        public AppointmentStatus Status { get; set; }
        public Guid ExpertId { get; set; }
    }

    public class SubscriptionBasicInfo
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string PackageType { get; set; } = string.Empty;
        public int Price { get; set; }
        public bool IsActive { get; set; }
    }
}
