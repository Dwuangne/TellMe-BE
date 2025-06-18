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
    public class SubscriptionPackage
    {
        public SubscriptionPackage()
        {
            CreatedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
            DurationUnit = DurationUnit.Month;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string PackageName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        [MaxLength(10)]
        public DurationUnit DurationUnit { get; set; } 

        public string? Features { get; set; }

        public string PackageType { get; set; } = string.Empty;

        public int Price { get; set; }
        public int PromotionId { get; set; } = 0;
        public int PromotionCount { get; set; } = 0;

        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual Promotion? Promotion { get; set; }
    }
}
