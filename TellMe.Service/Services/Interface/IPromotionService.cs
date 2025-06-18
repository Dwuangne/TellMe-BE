using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Service.Models.RequestModels;

namespace TellMe.Service.Services.Interface
{
    public interface IPromotionService
    {
        Task<Promotion> GetByIdAsync(int id);
        Task<IEnumerable<Promotion>> GetAllAsync();
        Task<IEnumerable<Promotion>> GetActivePromotionsAsync();
        Task<Promotion> CreateAsync(PromotionRequest promotion);
        Task<Promotion> UpdateAsync(int id, PromotionRequest promotion);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsPromotionValidAsync(int promotionId);
    }
}
