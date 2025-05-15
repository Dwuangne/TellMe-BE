using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Repository.Infrastructures;
using TellMe.Service.Models.ResponseModels;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class PsychologistReviewService : IPsychologistReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public PsychologistReviewService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<PsychologistReviewResponse> AddReviewAsync(Guid userId, Guid expertId, byte rating, string comment)
        {
            // Validate user exists
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new ArgumentException("User not found");

            var psychologist = await _userManager.FindByIdAsync(expertId.ToString());
            if (psychologist == null)
                throw new ArgumentException("Psychologist not found");

            var review = new PsychologistReview
            {
                UserId = userId,
                ExpertId = expertId,
                Rating = rating,
                Comment = comment,
                IsActive = true
            };

            await _unitOfWork.PsychologistReviewRepository.AddAsync(review);
            await _unitOfWork.CommitAsync();

            return await MapToResponseAsync(review);
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            var review = await _unitOfWork.PsychologistReviewRepository.GetByIdAsync(reviewId);
            if (review == null || !review.IsActive)
                return false;

            review.IsActive = false;
            review.ReviewDateUpdate = DateTime.Now;

            _unitOfWork.PsychologistReviewRepository.Update(review);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> RestoreReviewAsync(int reviewId)
        {
            var review = await _unitOfWork.PsychologistReviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                return false;

            review.IsActive = true;
            review.ReviewDateUpdate = DateTime.Now;

            _unitOfWork.PsychologistReviewRepository.Update(review);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<PsychologistReviewResponse?> GetReviewByIdAsync(int reviewId)
        {
            var review = await _unitOfWork.PsychologistReviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                return null;

            return await MapToResponseAsync(review);
        }

        public async Task<(IEnumerable<PsychologistReviewResponse> Reviews, int TotalPages, int TotalRecords)>
            GetReviewsActiveByPsychologistIdAsync(Guid expertId, int pageIndex, int pageSize)
        {
            var (reviews, totalPages, totalRecords) = await _unitOfWork.PsychologistReviewRepository.GetAsync(
                filter: r => r.ExpertId == expertId && r.IsActive,
                orderBy: q => q.OrderByDescending(r => r.ReviewDate),
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var reviewResponses = new List<PsychologistReviewResponse>();
            foreach (var review in reviews)
            {
                reviewResponses.Add(await MapToResponseAsync(review));
            }

            return (reviewResponses, totalPages, totalRecords);
        }

        public async Task<PsychologistReviewResponse> UpdateReviewAsync(int reviewId, byte rating, string comment)
        {
            var review = await _unitOfWork.PsychologistReviewRepository.GetByIdAsync(reviewId);
            if (review == null || !review.IsActive)
                throw new ArgumentException("Review not found");

            review.Rating = rating;
            review.Comment = comment;
            review.ReviewDateUpdate = DateTime.Now;

            _unitOfWork.PsychologistReviewRepository.Update(review);
            await _unitOfWork.CommitAsync();

            return await MapToResponseAsync(review);
        }

        private async Task<PsychologistReviewResponse> MapToResponseAsync(PsychologistReview review)
        {
            var response = _mapper.Map<PsychologistReviewResponse>(review);

            // Get user name
            var user = await _userManager.FindByIdAsync(review.UserId.ToString());
            if (user != null)
            {
                response.UserName = user.FullName;
            }

            // Get psychologist name 
            var psychologist = await _userManager.FindByIdAsync(review.ExpertId.ToString());
            if (psychologist != null)
            {
                response.ExpertName = psychologist.FullName;
            }

            return response;
        }

        public async Task<(IEnumerable<PsychologistReviewResponse> Reviews, int TotalPages, int TotalRecords)> 
            GetReviewsByPsychologistIdAsync(Guid? expertId, int pageIndex, int pageSize)
        {
            var (reviews, totalPages, totalRecords) = await _unitOfWork.PsychologistReviewRepository.GetAsync(
                filter:  expertId.HasValue ? r => r.ExpertId == expertId : null,
                orderBy: q => q.OrderByDescending(r => r.ReviewDate),
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var reviewResponses = new List<PsychologistReviewResponse>();
            foreach (var review in reviews)
            {
                reviewResponses.Add(await MapToResponseAsync(review));
            }

            return (reviewResponses, totalPages, totalRecords);
        }
    }
}
