using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
    public class UserPromotionService : IUserPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITimeHelper _timeHelper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserPromotionService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, ITimeHelper timeHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _timeHelper = timeHelper;
        }

        public async Task<UserPromotion> GetUserPromotionByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid ID", nameof(id));

            return await _unitOfWork.UserPromotionRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<UserPromotion>> GetAllUserPromotionsAsync()
        {
            var userPromotions = await _unitOfWork.UserPromotionRepository.GetAllAsync();

            // Load promotion details for each user promotion
            foreach (var userPromotion in userPromotions)
            {
                userPromotion.Promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(userPromotion.PromotionId);
            }

            return userPromotions;
        }

        public async Task<IEnumerable<UserPromotion>> GetUserPromotionsByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            // Verify the user exists
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var userPromotions = await _unitOfWork.UserPromotionRepository.GetAllAsync();

            // Include related promotion details if needed
            var userPromotionsWithDetails = userPromotions
                .Where(up => up.UserId == userId)
                .ToList();

            // Load promotion details for each user promotion
            foreach (var userPromotion in userPromotionsWithDetails)
            {
                userPromotion.Promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(userPromotion.PromotionId);
            }

            return userPromotionsWithDetails;
        }

        public async Task<UserPromotion> AddUserPromotionAsync(UserPromotionRequest userPromotionRequest)
        {
            if (userPromotionRequest == null)
                throw new ArgumentNullException(nameof(userPromotionRequest));

            var userId = new Guid(userPromotionRequest.UserId.ToString());
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(userPromotionRequest.UserId));

            if (userPromotionRequest.PromotionId <= 0)
                throw new ArgumentException("Invalid promotion ID", nameof(userPromotionRequest.PromotionId));

            // Verify the user exists
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            // Verify if the promotion exists and is active
            var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(userPromotionRequest.PromotionId);
            if (promotion == null)
                throw new KeyNotFoundException($"Promotion with ID {userPromotionRequest.PromotionId} not found");

            if (!promotion.IsActive)
                throw new InvalidOperationException($"Promotion with ID {userPromotionRequest.PromotionId} is not active");

            // Check if promotion is still valid (within date range)
            var currentDateTime = _timeHelper.NowVietnam();
            //if (promotion.StartDate > currentDateTime || promotion.EndDate < currentDateTime)
            //    throw new InvalidOperationException($"Promotion with ID {userPromotionRequest.PromotionId} is not currently valid");

            // Check if user already has this promotion
            var existingUserPromotions = await _unitOfWork.UserPromotionRepository.GetAllAsync();
            var existingPromotion = existingUserPromotions.FirstOrDefault(up =>
                up.UserId == userId &&
                up.PromotionId == userPromotionRequest.PromotionId);

            UserPromotion result;
            if (existingPromotion != null)
            {
                // Update existing promotion count
                existingPromotion.PromotionCount += userPromotionRequest.PromotionCount;
                _unitOfWork.UserPromotionRepository.Update(existingPromotion);
                await _unitOfWork.CommitAsync();
                result = existingPromotion;
            }
            else
            {
                // Create new user promotion
                var userPromotion = _mapper.Map<UserPromotion>(userPromotionRequest);
                userPromotion.UserId = userId; // Ensure the correct Guid is set
                await _unitOfWork.UserPromotionRepository.AddAsync(userPromotion);
                await _unitOfWork.CommitAsync();
                result = userPromotion;
            }

            return result;
        }

        public async Task<bool> UpdateUserPromotionAsync(int id, UserPromotionRequest userPromotionRequest)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid ID", nameof(id));

            if (userPromotionRequest == null)
                throw new ArgumentNullException(nameof(userPromotionRequest));

            var userId = new Guid(userPromotionRequest.UserId.ToString());
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(userPromotionRequest.UserId));

            if (userPromotionRequest.PromotionId <= 0)
                throw new ArgumentException("Invalid promotion ID", nameof(userPromotionRequest.PromotionId));

            try
            {
                // Get the user promotion by ID
                var existingUserPromotion = await _unitOfWork.UserPromotionRepository.GetByIdAsync(id);
                if (existingUserPromotion == null)
                    return false;

                // Verify the user exists
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return false;

                // Verify the promotion exists
                var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(userPromotionRequest.PromotionId);
                if (promotion == null)
                    return false;

                // Update user promotion properties
                existingUserPromotion.UserId = userId;
                existingUserPromotion.PromotionId = userPromotionRequest.PromotionId;
                existingUserPromotion.PromotionCount = userPromotionRequest.PromotionCount;

                _unitOfWork.UserPromotionRepository.Update(existingUserPromotion);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserPromotionAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid ID", nameof(id));

            try
            {
                // Check if the user promotion exists
                var userPromotion = await _unitOfWork.UserPromotionRepository.GetByIdAsync(id);
                if (userPromotion == null)
                    return false;

                // Delete the user promotion
                await _unitOfWork.UserPromotionRepository.DeleteAsync(id);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
