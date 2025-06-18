using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Service.Models.RequestModels;

namespace TellMe.Service.Services.Interface
{
    public interface IUserPromotionService
    {
        Task<UserPromotion> GetUserPromotionByIdAsync(int id);
        Task<IEnumerable<UserPromotion>> GetUserPromotionsByUserIdAsync(Guid userId);
        Task<IEnumerable<UserPromotion>> GetAllUserPromotionsAsync(); 
        Task<UserPromotion> AddUserPromotionAsync(UserPromotionRequest userPromotion);
        Task<bool> UpdateUserPromotionAsync(int Id, UserPromotionRequest userPromotion);
        Task<bool> DeleteUserPromotionAsync(int id);
    }
}
