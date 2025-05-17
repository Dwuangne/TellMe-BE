using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enums;

namespace TellMe.Service.Models.ResponseModels
{
    public class UserSubscriptionResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int PackageId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string? PackageDescription { get; set; }

        public string? Features { get; set; }
        public int Duration { get; set; }
        public DurationUnit DurationUnit { get; set; }
        public PaymentResponse? Payment { get; set; }

        public bool IsPaid { get; set; } = false;
    }
}
