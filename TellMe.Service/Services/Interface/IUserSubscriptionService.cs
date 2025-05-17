using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;

namespace TellMe.Service.Services.Interface
{
    public interface IUserSubscriptionService
    {
        Task<UserSubscriptionResponse> CreateSubscriptionAsync(Guid userId, CreateUserSubscriptionRequest request);
        Task<UserSubscriptionResponse?> GetSubscriptionByIdAsync(Guid id);
        Task<IEnumerable<UserSubscriptionResponse>> GetUserSubscriptionsAsync(Guid userId);
        Task<UserSubscriptionResponse> UpdateSubscriptionAsync(Guid id, UpdateUserSubscriptionRequest request);
        Task<bool> CancelSubscriptionAsync(Guid id);
        Task<bool> IsSubscriptionActiveAsync(Guid userId);
        Task<IEnumerable<UserSubscriptionResponse>> GetAllSubscriptionAsync(Guid? userId = null);
        Task<UserSubscriptionResponse?> GetActiveSubscriptionAsync(Guid userId);
        Task<bool> UpdatePaymentIdAfterPaymentAsync(Guid? subscriptionId, Guid paymentId);
    }
}
