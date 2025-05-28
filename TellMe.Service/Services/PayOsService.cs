using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.DBContexts;
using TellMe.Repository.Enities;
using TellMe.Repository.Enums;
using TellMe.Repository.Infrastructures;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class PayOsService : IPayOsService
    {
        private readonly PayOS _payOS;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public PayOsService(IConfiguration configuration, IUnitOfWork unitOfWork, IUserSubscriptionService userSubscriptionService, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _userSubscriptionService = userSubscriptionService;
            _userManager = userManager;
            _mapper = mapper;
            _payOS = new PayOS(
                _configuration["CheckOutPayOs:PAYOS_CLIENT_ID"],
                _configuration["CheckOutPayOs:PAYOS_API_KEY"],
                _configuration["CheckOutPayOs:PAYOS_CHECKSUM_KEY"]
            );
        }

        public async Task<string> CreatePaymentUrl(CreatePaymentRequest model, HttpContext context)
        {
            //create payment
            var paymentId = Guid.NewGuid();
            Payment payment = null;
            
            if (model.PaymentId.HasValue)
            {
                var paymentCheck = await _unitOfWork.PaymentRepository.GetByIdAsync(model.PaymentId);
                if (paymentCheck == null)
                {
                    payment = new Payment()
                    {
                        Id = paymentId,
                        UserId = model.UserId,
                        Amount = model.Amount,
                        AppointmentId = model.AppointmentId.HasValue ? model.AppointmentId : null,
                        UserSubscriptionId = model.UserSubscriptionId.HasValue ? model.UserSubscriptionId : null,
                        PaymentMethod = model.PaymentMethod,
                        Status = Repository.Enums.PaymentStatus.Pending
                    };
                    await _unitOfWork.PaymentRepository.AddAsync(payment);
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    paymentId = paymentCheck.Id;
                    payment = paymentCheck;
                }
            }
            else
            {
                payment = new Payment()
                {
                    Id = paymentId,
                    UserId = model.UserId,
                    Amount = model.Amount,
                    AppointmentId = model.AppointmentId.HasValue ? model.AppointmentId : null,
                    UserSubscriptionId = model.UserSubscriptionId.HasValue ? model.UserSubscriptionId : null,
                    PaymentMethod = model.PaymentMethod,
                    Status = Repository.Enums.PaymentStatus.Pending
                };
                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.CommitAsync();
            }

            // Lấy orderCode dạng long (dùng ticks hoặc hash từ paymentId)
            long orderCode = Math.Abs(paymentId.GetHashCode());

            // Cập nhật OrderCode vào Payment entity
            payment.OrderCode = orderCode;
            _unitOfWork.PaymentRepository.Update(payment);
            await _unitOfWork.CommitAsync();

            // Đọc URL hủy và URL hoàn tất từ cấu hình
            var cancelUrl = _configuration["CheckOutPayOs:CANCEL_URL"];
            var returnUrl = _configuration["CheckOutPayOs:RETURN_URL"];

            // Tạo danh sách item (ví dụ đơn giản, bạn có thể mở rộng lấy từ DB)
            var items = new List<ItemData>
            {
                new ItemData(
                    model.AppointmentId.HasValue ? "Appointment Payment" : "Subscription Payment",
                    1,
                    (int)model.Amount
                )
            };

            // Tạo dữ liệu thanh toán
            var paymentData = new PaymentData(
                orderCode,
                (int)model.Amount,
                $"Thanh toan TellMe",
                items,
                cancelUrl,
                returnUrl
            );

            // Tạo đường dẫn thanh toán
            var createPayment = await _payOS.createPaymentLink(paymentData);
            return createPayment.checkoutUrl;
        }

        public async Task<PaymentResponse> PaymentExecute(IQueryCollection collections)
        {
            try
            {
                // Lấy orderCode từ query string (PayOS callback trả về)
                var orderCodeStr = collections["orderCode"].ToString();
                if (string.IsNullOrEmpty(orderCodeStr) || !long.TryParse(orderCodeStr, out var orderCode))
                {
                    throw new Exception("Invalid orderCode from PayOS callback");
                }

                // Tìm payment theo orderCode đã lưu trong DB
                var payments = await _unitOfWork.PaymentRepository.GetAllAsync();
                var payment = payments.FirstOrDefault(p => p.OrderCode == orderCode);
                if (payment == null)
                {
                    throw new Exception("Payment not found");
                }

                // Lấy status và transactionId từ query string
                var status = collections["status"].ToString();
                var transactionId = collections["transactionId"].ToString();
                payment.TransactionId = transactionId;

                if (status == "PAID")
                {
                    payment.Status = PaymentStatus.Success;
                    _unitOfWork.PaymentRepository.Update(payment);

                    if (payment.AppointmentId.HasValue)
                    {
                        var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(payment.AppointmentId.Value);
                        if (appointment != null && appointment.Status == AppointmentStatus.Pending)
                        {
                            appointment.Status = AppointmentStatus.Confirmed;
                            appointment.IsPaid = true;
                            appointment.UpdatedAt = DateTime.Now;
                            appointment.PaymentId = payment.Id;
                            appointment.UserId = payment.UserId;
                            _unitOfWork.AppointmentRepository.Update(appointment);
                        }
                    }
                    else if (payment.UserSubscriptionId.HasValue)
                    {
                        var subscription = await _unitOfWork.UserSubscriptionRepository.GetByIdAsync(payment.UserSubscriptionId.Value);
                        if (subscription != null)
                        {
                            subscription.IsActive = true;
                            subscription.IsPaid = true;
                            subscription.PaymentId = payment.Id;
                            subscription.UserId = (Guid)payment.UserId;
                            _unitOfWork.UserSubscriptionRepository.Update(subscription);
                        }
                    }

                    await _unitOfWork.CommitAsync();
                }
                else if (status == "CANCELLED" || status == "EXPIRED")
                {
                    payment.Status = PaymentStatus.Failed;
                    _unitOfWork.PaymentRepository.Update(payment);
                    await _unitOfWork.CommitAsync();
                }

                return await MapToResponseAsync(payment);
            }
            catch (Exception ex)
            {
                throw new Exception("Payment execution failed", ex);
            }
        }

        private async Task<PaymentResponse> MapToResponseAsync(Payment payment)
        {
            var response = new PaymentResponse
            {
                Id = payment.Id,
                UserId = (Guid)payment.UserId,
                AppointmentId = payment.AppointmentId,
                UserSubscriptionId = payment.UserSubscriptionId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                OrderCode = payment.OrderCode,
                TransactionId = payment.TransactionId,
                PaymentDate = payment.PaymentDate,
                IsActive = payment.IsActive
            };

            // Load appointment details if exists
            if (payment.AppointmentId.HasValue)
            {
                var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(payment.AppointmentId.Value);
                if (appointment != null)
                {
                    response.AppointmentInfo = new AppointmentBasicInfo
                    {
                        Id = appointment.Id,
                        AppointmentDateTime = appointment.AppointmentDateTime,
                        Fee = appointment.Fee,
                        Status = appointment.Status,
                        ExpertId = appointment.ExpertId
                    };
                }
            }

            // Load subscription details if exists
            if (payment.UserSubscriptionId.HasValue)
            {
                var subscription = await _unitOfWork.UserSubscriptionRepository.GetByIdAsync(payment.UserSubscriptionId.Value);
                if (subscription != null)
                {
                    var package = await _unitOfWork.SubscriptionPackageRepository.GetByIdAsync(subscription.PackageId);
                    response.SubscriptionInfo = new SubscriptionBasicInfo
                    {
                        Id = subscription.Id,
                        StartDate = subscription.StartDate,
                        EndDate = subscription.EndDate,
                        PackageName = package?.PackageName ?? "Unknown Package",
                        Price = package?.Price ?? 0,
                        PackageType = package?.PackageType ?? "Unknown",
                        IsActive = subscription.IsActive
                    };
                }
            }

            if (payment.UserId.HasValue)
            {
                var user = await _userManager.FindByIdAsync(payment.UserId.Value.ToString());
                if (user != null)
                {
                    response.User = _mapper.Map<ProfileResponse>(user);
                }
            }

            return response;
        }
    }
}
