using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Repository.Infrastructures;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITimeHelper _timeHelper;

        public PromotionService(IUnitOfWork unitOfWork, IMapper mapper, ITimeHelper timeHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _timeHelper = timeHelper;
        }

        public async Task<Promotion> GetByIdAsync(int id)
        {
            return await _unitOfWork.PromotionRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Promotion>> GetAllAsync()
        {
            return await _unitOfWork.PromotionRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
        {
            var allPromotions = await _unitOfWork.PromotionRepository.GetAllAsync();
            var currentDate = _timeHelper.NowVietnam();

            return allPromotions.Where(p =>
                p.IsActive &&
                p.StartDate <= currentDate &&
                p.EndDate >= currentDate);
        }

        public async Task<Promotion> CreateAsync(PromotionRequest promotionRequest)
        {
            if (promotionRequest == null)
                throw new ArgumentNullException(nameof(promotionRequest));

            var promotion = _mapper.Map<Promotion>(promotionRequest);

            promotion.StartDate = promotionRequest.StartDate == null ? _timeHelper.NowVietnam() : promotionRequest.StartDate;
            promotion.EndDate = promotionRequest.EndDate == null ? _timeHelper.NowVietnam().AddDays(90) : promotionRequest.EndDate; 

            await _unitOfWork.PromotionRepository.AddAsync(promotion);
            await _unitOfWork.CommitAsync();

            return promotion;
        }

        public async Task<Promotion> UpdateAsync(int Id, PromotionRequest promotionRequest)
        {
            if (promotionRequest == null)
                throw new ArgumentException("Invalid promotion data", nameof(promotionRequest));

            var existingPromotion = await _unitOfWork.PromotionRepository.GetByIdAsync(Id);
            if (existingPromotion == null)
                throw new KeyNotFoundException($"Promotion with ID {Id} not found");

            // Update properties from request
            _mapper.Map(promotionRequest, existingPromotion);

            // Preserve original dates if they're not in the request
            if (promotionRequest.StartDate == default)
                existingPromotion.StartDate = existingPromotion.StartDate;

            if (promotionRequest.EndDate == default)
                existingPromotion.EndDate = existingPromotion.EndDate;

            _unitOfWork.PromotionRepository.Update(existingPromotion);
            await _unitOfWork.CommitAsync();

            return existingPromotion;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(id);
            if (promotion == null)
                return false;

            promotion.IsActive = false; 
            _unitOfWork.PromotionRepository.Update(promotion);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> IsPromotionValidAsync(int promotionId)
        {
            var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(promotionId);
            if (promotion == null)
                return false;

            var currentDate = _timeHelper.NowVietnam();
            return promotion.IsActive &&
                   promotion.StartDate <= currentDate &&
                   promotion.EndDate >= currentDate;
        }
    }
}
