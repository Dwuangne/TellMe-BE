using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Enities
{
    public class UserSubscription
    {
        public UserSubscription()
        {
            Id = Guid.NewGuid();
            StartDate = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int PackageId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public bool IsPaid { get; set; } = false;

        [ForeignKey("PackageId")]
        public virtual SubscriptionPackage Package { get; set; }

        public Guid? PaymentId { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment? Payment { get; set; }

    }
}
