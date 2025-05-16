using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enums;

namespace TellMe.Service.Models.RequestModels
{
    public class CreatePackageRequest
    {
        [Required]
        [StringLength(255)]
        public string PackageName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Duration { get; set; }

        [Required]
        public DurationUnit DurationUnit { get; set; }

        public string? Features { get; set; }

        public int Price { get; set; }
    }

    public class UpdatePackageRequest : CreatePackageRequest
    {
        public bool IsActive { get; set; } = true;
    }
}
