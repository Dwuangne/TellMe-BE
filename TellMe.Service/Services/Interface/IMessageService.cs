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
    public interface IMessageService
    {
        Task<PaginatedResponse<MessageResponse>> GetMessagesByConversationIdAsync(Guid conversationId, int pageIndex = 1, int pageSize = 20);
        Task<MessageResponse> GetMessageByIdAsync(Guid messageId);
        Task<MessageResponse> AddMessageAsync(MessageRequest messageRequest);
        Task<bool> DeleteMessageAsync(Guid messageId, Guid userId);
    }
} 