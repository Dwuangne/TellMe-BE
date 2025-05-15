using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Service.Models.ResponseModels;

namespace TellMe.Service.Services.Interface
{
    public interface IPsychologistReviewService
    {
        Task<PsychologistReviewResponse> AddReviewAsync(Guid userId, Guid expertId, byte rating, string comment);
        Task<PsychologistReviewResponse> UpdateReviewAsync(int reviewId, byte rating, string comment);
        Task<bool> DeleteReviewAsync(int reviewId);
        Task<(IEnumerable<PsychologistReviewResponse> Reviews, int TotalPages, int TotalRecords)>
        GetReviewsByPsychologistIdAsync(Guid? expertId, int pageIndex, int pageSize);
        Task<(IEnumerable<PsychologistReviewResponse> Reviews, int TotalPages, int TotalRecords)>
        GetReviewsActiveByPsychologistIdAsync(Guid expertId, int pageIndex, int pageSize);
        Task<PsychologistReviewResponse?> GetReviewByIdAsync(int reviewId);
        Task<bool> RestoreReviewAsync(int reviewId);
    }
}
