using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Enities
{
    public class PsychologistReview
    {
        public PsychologistReview()
        {
            ReviewDate = DateTime.Now;
            ReviewDateUpdate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ExpertId { get; set; }

        [Range(0, 5)]
        public byte? Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime ReviewDate { get; set; }

        public DateTime ReviewDateUpdate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
