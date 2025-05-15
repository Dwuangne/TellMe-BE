using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Repository.Enums;
using TellMe.Repository.Infrastructures;
using TellMe.Service.Models.ResponseModels;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CancelPaymentAsync(Guid id)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
            if (payment == null || !payment.IsActive)
                return false;

            if (payment.Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending payments can be cancelled");

            payment.IsActive = false;
            payment.Status = PaymentStatus.Failed;

            _unitOfWork.PaymentRepository.Update(payment);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<IEnumerable<PaymentResponse>> GetAllPaymentsAsync(Guid? userId = null)
        {
            var payments = await _unitOfWork.PaymentRepository.GetAsync(
                filter: userId.HasValue
                    ? p => p.UserId == userId.Value 
                    : null,
                orderBy: q => q.OrderByDescending(p => p.PaymentDate)
            );

            var responses = new List<PaymentResponse>();
            foreach (var payment in payments.Items)
            {
                responses.Add(await MapToPaymentResponse(payment));
            }

            return responses;
        }

        public async Task<PaymentResponse?> GetPaymentByIdAsync(Guid id)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
            if (payment == null)
                return null;

            return await MapToPaymentResponse(payment);
        }

        public async Task<IEnumerable<PaymentResponse>> GetUserPaymentsAsync(Guid userId)
        {
            var payments = await _unitOfWork.PaymentRepository.GetAsync(
                filter: p => p.UserId == userId && p.IsActive,
                orderBy: q => q.OrderByDescending(p => p.PaymentDate)
            );

            var responses = new List<PaymentResponse>();
            foreach (var payment in payments.Items)
            {
                responses.Add(await MapToPaymentResponse(payment));
            }

            return responses;
        }

        private async Task<PaymentResponse> MapToPaymentResponse(Payment payment)
        {
            var response = _mapper.Map<PaymentResponse>(payment);

            return response;
        }
    }
}
