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
    public class ParticipantService : IParticipantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ParticipantService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<ParticipantResponse>> GetParticipantsByConversationIdAsync(Guid conversationId, int pageIndex = 1, int pageSize = 20)
        {
            // Kiểm tra conversation có tồn tại
            var conversationExists = await _unitOfWork.ConversationRepository.ExistsAsync(c => c.Id == conversationId);
            if (!conversationExists)
                throw new NotFoundException("Không tìm thấy cuộc trò chuyện");

            var result = await _unitOfWork.ParticipantRepository.GetAsync(
                filter: p => p.ConversationId == conversationId,
                orderBy: p => p.OrderBy(x => x.JoinedAt), // Order by join time
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var participantResponses = _mapper.Map<List<ParticipantResponse>>(result.Items);

            // TODO: Load FullName from User table if needed
            // foreach (var participant in participantResponses)
            // {
            //     // Load user info to get FullName
            // }

            return new PaginatedResponse<ParticipantResponse>
            {
                Items = participantResponses,
                PageIndex = pageIndex,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };
        }

        public async Task<ParticipantResponse> GetParticipantByIdAsync(Guid participantId)
        {
            var participant = await _unitOfWork.ParticipantRepository.FirstOrDefaultAsync(p => p.Id == participantId);

            if (participant == null)
                throw new NotFoundException("Không tìm thấy thành viên");

            var response = _mapper.Map<ParticipantResponse>(participant);
            
            // TODO: Load FullName from User table if needed
            
            return response;
        }

        public async Task<ParticipantResponse> AddParticipantAsync(ParticipantRequest participantRequest)
        {
            if (participantRequest.ConversationId == null)
                throw new BadRequestException("ConversationId là bắt buộc");

            var conversation = await _unitOfWork.ConversationRepository.FirstOrDefaultAsync(
                c => c.Id == participantRequest.ConversationId);

            if (conversation == null)
                throw new NotFoundException("Không tìm thấy cuộc trò chuyện");

            // Check if user is already participant
            var existingParticipant = await _unitOfWork.ParticipantRepository.FirstOrDefaultAsync(
                p => p.ConversationId == participantRequest.ConversationId && p.UserId == participantRequest.UserId);

            if (existingParticipant != null)
                throw new BadRequestException("Người dùng đã là thành viên của cuộc trò chuyện");

            var participant = _mapper.Map<Participant>(participantRequest);
            participant.Id = Guid.NewGuid();
            participant.JoinedAt = DateTime.Now;

            await _unitOfWork.ParticipantRepository.AddAsync(participant);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ParticipantResponse>(participant);
        }

        public async Task<bool> RemoveParticipantAsync(Guid conversationId, Guid userId)
        {
            var participant = await _unitOfWork.ParticipantRepository.FirstOrDefaultAsync(
                p => p.ConversationId == conversationId && p.UserId == userId);

            if (participant == null)
                throw new NotFoundException("Không tìm thấy thành viên trong cuộc trò chuyện");

            _unitOfWork.ParticipantRepository.Delete(participant);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> IsUserParticipantAsync(Guid conversationId, Guid userId)
        {
            return await _unitOfWork.ParticipantRepository.ExistsAsync(
                p => p.ConversationId == conversationId && p.UserId == userId);
        }
    }
} 