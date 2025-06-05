using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;

namespace TellMe.Service.Services.Interface
{
    public interface IConversationService
    {
        Task<PaginatedResponse<ConversationResponse>> GetAllConversationAsync(int pageIndex = 1, int pageSize = 12);
        Task<ConversationResponse?> GetConversationByIdAsync(Guid conversationId, bool includeMessages = false, int messagePageIndex = 1, int messagePageSize = 20);
        Task<PaginatedResponse<ConversationResponse>> GetConversationByUserIdAsync(Guid userId, int pageIndex = 1, int pageSize = 12);
        Task<ConversationResponse> AddConversationAsync(ConversationRequest conversationRequest);
        Task<bool> DeleteConversationAsync(Guid conversationId, Guid userId);
    }
}
