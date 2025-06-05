using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Repository.Infrastructures;
using TellMe.Service.Exceptions;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ConversationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<ConversationResponse>> GetAllConversationAsync(int pageIndex = 1, int pageSize = 12)
        {
            var result = await _unitOfWork.ConversationRepository.GetAsync(
                orderBy: c => c.OrderByDescending(x => x.CreatedAt),
                includeProperties: "Participants",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var conversationResponses = new List<ConversationResponse>();
            foreach (var conversation in result.Items)
            {
                var response = _mapper.Map<ConversationResponse>(conversation);
                // Lazy load: Không load messages ở đây để tối ưu hiệu suất
                response.Messages = new List<MessageResponse>();
                conversationResponses.Add(response);
            }

            return new PaginatedResponse<ConversationResponse>
            {
                Items = conversationResponses,
                PageIndex = pageIndex,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };
        }

        public async Task<ConversationResponse?> GetConversationByIdAsync(Guid conversationId, bool includeMessages = false, int messagePageIndex = 1, int messagePageSize = 20)
        {
            var conversation = await _unitOfWork.ConversationRepository.FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
                throw new NotFoundException("Không tìm thấy cuộc trò chuyện");

            var response = _mapper.Map<ConversationResponse>(conversation);

            // Load participants
            var participants = await _unitOfWork.ParticipantRepository.GetAsync(
                filter: p => p.ConversationId == conversationId,
                orderBy: p => p.OrderBy(x => x.JoinedAt)
            );
            response.Participants = _mapper.Map<List<ParticipantResponse>>(participants.Items);

            // Lazy load messages only if requested
            if (includeMessages)
            {
                var messages = await _unitOfWork.MessageRepository.GetAsync(
                    filter: m => m.ConversationId == conversationId,
                    orderBy: m => m.OrderBy(x => x.SendAt),
                    pageIndex: messagePageIndex,
                    pageSize: messagePageSize
                );
                response.Messages = _mapper.Map<List<MessageResponse>>(messages.Items);
            }
            else
            {
                response.Messages = new List<MessageResponse>();
            }

            return response;
        }

        public async Task<PaginatedResponse<ConversationResponse>> GetConversationByUserIdAsync(Guid userId, int pageIndex = 1, int pageSize = 12)
        {
            // Lấy danh sách conversation IDs mà user tham gia
            var participantConversations = await _unitOfWork.ParticipantRepository.GetAsync(
                filter: p => p.UserId == userId
            );

            var conversationIds = participantConversations.Items.Select(p => p.ConversationId).ToList();

            if (!conversationIds.Any())
            {
                return new PaginatedResponse<ConversationResponse>
                {
                    Items = new List<ConversationResponse>(),
                    PageIndex = pageIndex,
                    TotalPages = 0,
                    TotalRecords = 0
                };
            }

            var conversations = await _unitOfWork.ConversationRepository.GetAsync(
                filter: c => conversationIds.Contains(c.Id),
                orderBy: c => c.OrderByDescending(x => x.CreatedAt),
                includeProperties: "Participants",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var conversationResponses = new List<ConversationResponse>();
            foreach (var conversation in conversations.Items)
            {
                var response = _mapper.Map<ConversationResponse>(conversation);
                // Lazy load: Không load messages ở đây để tối ưu hiệu suất
                response.Messages = new List<MessageResponse>();
                conversationResponses.Add(response);
            }

            return new PaginatedResponse<ConversationResponse>
            {
                Items = conversationResponses,
                PageIndex = pageIndex,
                TotalPages = conversations.TotalPages,
                TotalRecords = conversations.TotalRecords
            };
        }

        public async Task<ConversationResponse> AddConversationAsync(ConversationRequest conversationRequest)
        {
            var conversation = _mapper.Map<Conversation>(conversationRequest);
            conversation.Id = Guid.NewGuid();
            conversation.CreatedAt = DateTime.Now;

            // Clear collections để tránh mapping issues
            conversation.Participants.Clear();
            conversation.Messages.Clear();

            await _unitOfWork.ConversationRepository.AddAsync(conversation);

            // Handle participants
            foreach (var participantRequest in conversationRequest.Participants)
            {
                var participant = _mapper.Map<Participant>(participantRequest);
                participant.Id = Guid.NewGuid();
                participant.ConversationId = conversation.Id;
                participant.JoinedAt = DateTime.Now;
                await _unitOfWork.ParticipantRepository.AddAsync(participant);
            }

            // Handle messages
            foreach (var messageRequest in conversationRequest.Messages)
            {
                var message = _mapper.Map<Message>(messageRequest);
                message.Id = Guid.NewGuid();
                message.ConversationId = conversation.Id;
                message.SendAt = DateTime.Now;
                await _unitOfWork.MessageRepository.AddAsync(message);
            }

            await _unitOfWork.CommitAsync();

            // Return conversation with related data
            return await GetConversationByIdAsync(conversation.Id, includeMessages: true) ?? 
                   throw new InvalidOperationException("Failed to retrieve created conversation");
        }

        public async Task<bool> DeleteConversationAsync(Guid conversationId, Guid userId)
        {
            var conversation = await _unitOfWork.ConversationRepository.FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
                throw new NotFoundException("Không tìm thấy cuộc trò chuyện");

            // Check if user is participant
            var isParticipant = await _unitOfWork.ParticipantRepository.ExistsAsync(
                p => p.ConversationId == conversationId && p.UserId == userId);

            if (!isParticipant)
                throw new BadRequestException("Bạn không có quyền xóa cuộc trò chuyện này");

            _unitOfWork.ConversationRepository.Delete(conversation);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
