using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models.ResponseModels
{
    public class PsychologistReviewResponse
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ExpertId { get; set; }

        public byte? Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime ReviewDate { get; set; }

        public DateTime ReviewDateUpdate { get; set; }

        public bool IsActive { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string ExpertName { get; set; } = string.Empty;
    }
}
