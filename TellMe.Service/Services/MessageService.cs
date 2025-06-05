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
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MessageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<MessageResponse>> GetMessagesByConversationIdAsync(Guid conversationId, int pageIndex = 1, int pageSize = 20)
        {
            // Kiểm tra conversation có tồn tại
            var conversationExists = await _unitOfWork.ConversationRepository.ExistsAsync(c => c.Id == conversationId);
            if (!conversationExists)
                throw new NotFoundException("Không tìm thấy cuộc trò chuyện");

            var result = await _unitOfWork.MessageRepository.GetAsync(
                filter: m => m.ConversationId == conversationId,
                orderBy: m => m.OrderBy(x => x.SendAt), // Order by time sent
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var messageResponses = _mapper.Map<List<MessageResponse>>(result.Items);

            return new PaginatedResponse<MessageResponse>
            {
                Items = messageResponses,
                PageIndex = pageIndex,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };
        }

        public async Task<MessageResponse> GetMessageByIdAsync(Guid messageId)
        {
            var message = await _unitOfWork.MessageRepository.FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null)
                throw new NotFoundException("Không tìm thấy tin nhắn");

            return _mapper.Map<MessageResponse>(message);
        }

        public async Task<MessageResponse> AddMessageAsync(MessageRequest messageRequest)
        {
            if (messageRequest.ConversationId == null)
                throw new BadRequestException("ConversationId là bắt buộc");

            var conversation = await _unitOfWork.ConversationRepository.FirstOrDefaultAsync(
                c => c.Id == messageRequest.ConversationId);

            if (conversation == null)
                throw new NotFoundException("Không tìm thấy cuộc trò chuyện");

            // Check if user is participant
            var isParticipant = await _unitOfWork.ParticipantRepository.ExistsAsync(
                p => p.ConversationId == messageRequest.ConversationId && p.UserId == messageRequest.UserId);

            if (!isParticipant)
                throw new BadRequestException("Người dùng không thuộc cuộc trò chuyện này");

            var message = _mapper.Map<Message>(messageRequest);
            message.Id = Guid.NewGuid();
            message.SendAt = DateTime.Now;

            await _unitOfWork.MessageRepository.AddAsync(message);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<MessageResponse>(message);
        }

        public async Task<bool> DeleteMessageAsync(Guid messageId, Guid userId)
        {
            var message = await _unitOfWork.MessageRepository.FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null)
                throw new NotFoundException("Không tìm thấy tin nhắn");

            if (message.UserId != userId)
                throw new BadRequestException("Bạn không có quyền xóa tin nhắn này");

            _unitOfWork.MessageRepository.Delete(message);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
} 