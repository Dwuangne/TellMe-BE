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
    public interface IParticipantService
    {
        Task<PaginatedResponse<ParticipantResponse>> GetParticipantsByConversationIdAsync(Guid conversationId, int pageIndex = 1, int pageSize = 20);
        Task<ParticipantResponse> GetParticipantByIdAsync(Guid participantId);
        Task<ParticipantResponse> AddParticipantAsync(ParticipantRequest participantRequest);
        Task<bool> RemoveParticipantAsync(Guid conversationId, Guid userId);
        Task<bool> IsUserParticipantAsync(Guid conversationId, Guid userId);
    }
} 