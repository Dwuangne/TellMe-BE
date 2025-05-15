using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Repository.Enums;
using TellMe.Repository.Infrastructures;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserSubscriptionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserSubscriptionResponse> CreateSubscriptionAsync(Guid userId, CreateUserSubscriptionRequest request)
        {
            // Validate if user already has an active subscription
            var activeSubscription = await GetActiveSubscriptionAsync(userId);
            if (activeSubscription != null)
                throw new InvalidOperationException("User already has an active subscription");

            // Validate package exists and is active
            var package = await _unitOfWork.SubscriptionPackageRepository.GetByIdAsync(request.PackageId);
            if (package == null || !package.IsActive)
                throw new ArgumentException("Invalid or inactive package");

            // Validate payment
            //var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(request.PaymentId);
            //if (payment == null || payment.UserId != userId || !payment.IsActive)
            //    throw new ArgumentException("Invalid payment");

            var subscription = new UserSubscription
            {
                UserId = userId,
                PackageId = request.PackageId,
                StartDate = DateTime.Now,
                EndDate = CalculateEndDate(DateTime.Now, package.Duration, package.DurationUnit),
                IsActive = true
            };

            await _unitOfWork.UserSubscriptionRepository.AddAsync(subscription);
            await _unitOfWork.CommitAsync();

            return await MapToResponseAsync(subscription);
        }

        public async Task<bool> CancelSubscriptionAsync(Guid id)
        {
            var subscription = await _unitOfWork.UserSubscriptionRepository.GetByIdAsync(id);
            if (subscription == null || !subscription.IsActive)
                return false;

            subscription.IsActive = false;
            _unitOfWork.UserSubscriptionRepository.Update(subscription);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<UserSubscriptionResponse?> GetActiveSubscriptionAsync(Guid userId)
        {
            var subscription = await _unitOfWork.UserSubscriptionRepository
                .FirstOrDefaultAsync(s => s.UserId == userId &&
                                        s.IsActive &&
                                        s.EndDate > DateTime.Now);

            return subscription == null ? null : await MapToResponseAsync(subscription);
        }

        public async Task<UserSubscriptionResponse?> GetSubscriptionByIdAsync(Guid id)
        {
            var subscription = await _unitOfWork.UserSubscriptionRepository.GetByIdAsync(id);
            return subscription == null ? null : await MapToResponseAsync(subscription);
        }

        public async Task<IEnumerable<UserSubscriptionResponse>> GetAllSubscriptionAsync(Guid? userId = null)
        {
            var subscriptions = await _unitOfWork.UserSubscriptionRepository.GetAsync(
                filter: userId.HasValue ? s => s.UserId == userId : null,
                orderBy: q => q.OrderByDescending(s => s.StartDate)
                //includeProperties: "Package,Payment"
            );

            var responses = new List<UserSubscriptionResponse>();
            foreach (var subscription in subscriptions.Items)
            {
                responses.Add(await MapToResponseAsync(subscription));
            }

            return responses;
        }

        public async Task<IEnumerable<UserSubscriptionResponse>> GetUserSubscriptionsAsync(Guid userId)
        {
            var subscriptions = await _unitOfWork.UserSubscriptionRepository.GetAsync(
                filter: s => s.UserId == userId && s.IsActive,
                orderBy: q => q.OrderByDescending(s => s.StartDate)
                //includeProperties: "Package,Payment"
            );

            var responses = new List<UserSubscriptionResponse>();
            foreach (var subscription in subscriptions.Items)
            {
                responses.Add(await MapToResponseAsync(subscription));
            }

            return responses;
        }

        public async Task<bool> IsSubscriptionActiveAsync(Guid userId)
        {
            return await GetActiveSubscriptionAsync(userId) != null;
        }

        public async Task<UserSubscriptionResponse> UpdateSubscriptionAsync(Guid id, UpdateUserSubscriptionRequest request)
        {
            var subscription = await _unitOfWork.UserSubscriptionRepository.GetByIdAsync(id);
            if (subscription == null)
                throw new KeyNotFoundException("Subscription not found");

            subscription.IsActive = request.IsActive;
            _unitOfWork.UserSubscriptionRepository.Update(subscription);
            await _unitOfWork.CommitAsync();

            return await MapToResponseAsync(subscription);
        }

        public async Task<bool> UpdatePaymentIdAfterPaymentAsync(Guid subscriptionId, Guid paymentId)
        {
            var subscription = await _unitOfWork.UserSubscriptionRepository.GetByIdAsync(subscriptionId);
            if (subscription == null)
                throw new KeyNotFoundException("Subscription not found");
            subscription.PaymentId = paymentId;
            subscription.IsPaid = true;
            _unitOfWork.UserSubscriptionRepository.Update(subscription);
            await _unitOfWork.CommitAsync();
            return true;
        }

        private async Task<UserSubscriptionResponse> MapToResponseAsync(UserSubscription subscription)
        {
            var response = _mapper.Map<UserSubscriptionResponse>(subscription);

            // Load package details if not loaded
            if (subscription.Package == null)
            {
                var package = await _unitOfWork.SubscriptionPackageRepository.GetByIdAsync(subscription.PackageId);
                if (package != null)
                {
                    response.PackageName = package.PackageName;
                    response.PackageDescription = package.Description;
                    response.Duration = package.Duration;
                    response.DurationUnit = package.DurationUnit;
                }
            }
            else
            {
                response.PackageName = subscription.Package.PackageName;
                response.PackageDescription = subscription.Package.Description;
                response.Duration = subscription.Package.Duration;
                response.DurationUnit = subscription.Package.DurationUnit;
            }

            // Load payment details if available
            if (subscription.PaymentId.HasValue)
            {
                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(subscription.PaymentId.Value);
                if (payment != null)
                {
                    response.Payment = _mapper.Map<PaymentResponse>(payment);
                }
            }

            return response;
        }

        private DateTime CalculateEndDate(DateTime startDate, int duration, DurationUnit unit)
        {
            return unit switch
            {
                DurationUnit.Day => startDate.AddDays(duration),
                DurationUnit.Month => startDate.AddMonths(duration),
                DurationUnit.Year => startDate.AddYears(duration),
                _ => throw new ArgumentException("Invalid duration unit")
            };
        }
    }
}
