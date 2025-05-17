using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;

namespace TellMe.Service.Services.Interface
{
    public interface IVnPayService
    {
        Task<string> CreatePaymentUrl(CreatePaymentRequest model, HttpContext context);
        Task<PaymentResponse> PaymentExecute(IQueryCollection collections, string paymentId);

    }
}
