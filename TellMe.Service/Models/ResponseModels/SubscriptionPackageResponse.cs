using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Repository.Enums;

namespace TellMe.Service.Models.ResponseModels
{
    public class SubscriptionPackageResponse
    {
        public int Id { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Duration { get; set; }
        public DurationUnit DurationUnit { get; set; }
        public string? Features { get; set; }
        public string PackageType { get; set; } = string.Empty;
        public int Price { get; set; }
        public int PromotionId { get; set; }
        public int PromotionCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public Promotion? Promotion { get; set; } 
    }
}
