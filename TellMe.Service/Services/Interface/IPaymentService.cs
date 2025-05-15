using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enums;
using TellMe.Service.Models.ResponseModels;

namespace TellMe.Service.Services.Interface
{
    public interface IPaymentService
    {
        //Task<PaymentResponse> CreatePaymentAsync(Guid userId, CreatePaymentRequest request);
        Task<PaymentResponse?> GetPaymentByIdAsync(Guid id);
        Task<IEnumerable<PaymentResponse>> GetUserPaymentsAsync(Guid userId);
        Task<IEnumerable<PaymentResponse>> GetAllPaymentsAsync(Guid? userId = null);
        //Task<PaymentResponse> UpdatePaymentStatusAsync(Guid id, UpdatePaymentStatusRequest request);
        Task<bool> CancelPaymentAsync(Guid id);
    }
}
