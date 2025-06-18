using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.DBContexts;
using TellMe.Repository.Enities;
using TellMe.Repository.Enums;
using TellMe.Repository.Infrastructures;
using TellMe.Service.Libraries;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly ITimeHelper _timeHelper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public VnPayService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IUnitOfWork unitOfWork, IUserSubscriptionService userSubscriptionService, ITimeHelper timeHelper, IMapper mapper)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _userSubscriptionService = userSubscriptionService;
            _timeHelper = timeHelper;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<string> CreatePaymentUrl(CreatePaymentRequest model, HttpContext context)
        {
            //create payment
            var paymentId = Guid.NewGuid();
            if (model.PaymentId.HasValue)
            {
                var paymentCheck = await _unitOfWork.PaymentRepository.GetByIdAsync(model.PaymentId);
                if (paymentCheck == null)
                {

                    var payment = new Payment()
                    {
                        Id = paymentId,
                        UserId = model.UserId,
                        Amount = model.Amount,
                        AppointmentId = model.AppointmentId.HasValue ? model.AppointmentId : null,
                        UserSubscriptionId = model.UserSubscriptionId.HasValue ? model.UserSubscriptionId : null,
                        SubscriptionPackageId = model.SubscriptionPackageId.HasValue ? model.SubscriptionPackageId : null,
                        PaymentMethod = model.PaymentMethod,
                        Status = Repository.Enums.PaymentStatus.Pending,
                        PaymentDate = _timeHelper.NowVietnam()
                    };
                    await _unitOfWork.PaymentRepository.AddAsync(payment);
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    paymentId = paymentCheck.Id;
                }
            }
            else
            {
                var payment = new Payment()
                {
                    Id = paymentId,
                    UserId = model.UserId,
                    Amount = model.Amount,
                    AppointmentId = model.AppointmentId.HasValue ? model.AppointmentId : null,
                    UserSubscriptionId = model.UserSubscriptionId.HasValue ? model.UserSubscriptionId : null,
                    PaymentMethod = model.PaymentMethod,
                    Status = Repository.Enums.PaymentStatus.Pending,
                    PaymentDate = _timeHelper.NowVietnam()
                };
                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.CommitAsync();
            }
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["Vnpay:PaymentBackReturnUrl"];

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.UserId} {model.Amount}");
            pay.AddRequestData("vnp_OrderType", model.PaymentMethod);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", $"{paymentId}|{model.PromotionId}|{model.PromotionCount}|{tick}");

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        public async Task<PaymentResponse> PaymentExecute(IQueryCollection collections, string paymentId, string promotionId, string promotionCount)
        {
            try
            {
                var pay = new VnPayLibrary();
                foreach (var (key, value) in collections)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(key, value);
                    }
                }

                var vnPayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo"));
                var vnpResponseCode = pay.GetResponseData("vnp_ResponseCode");
                var vnpSecureHash = collections.FirstOrDefault(k => k.Key == "vnp_SecureHash").Value;
                var checkSignature = pay.ValidateSignature(vnpSecureHash, _configuration["Vnpay:HashSecret"]);

                if (!Guid.TryParse(paymentId, out var paymentGuid))
                {
                    throw new Exception("Invalid paymentId format");
                }


                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentGuid);
                if (payment == null)
                {
                    throw new Exception("Payment not found");
                }

                payment.TransactionId = vnPayTranId.ToString();

                if (!checkSignature)
                {
                    payment.Status = PaymentStatus.Failed;
                    _unitOfWork.PaymentRepository.Update(payment);
                    await _unitOfWork.CommitAsync();
                }
                else if (vnpResponseCode == "00")
                {
                    payment.Status = PaymentStatus.Success;
                    _unitOfWork.PaymentRepository.Update(payment);

                    if (payment.AppointmentId.HasValue)
                    {
                        // Process promotion if provided
                        if (!string.IsNullOrEmpty(promotionId) && int.TryParse(promotionId, out int promoId) && promoId > 0)
                        {
                            // Get promotion from repository
                            var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(promoId);
                            if (promotion != null && payment.UserId.HasValue)
                            {
                                // Find user promotion
                                var userPromotions = await _unitOfWork.UserPromotionRepository.GetAllAsync();
                                var userPromotion = userPromotions.FirstOrDefault(up =>
                                    up.UserId.Equals(payment.UserId.Value) &&
                                    up.PromotionId == promoId);

                                if (userPromotion != null && userPromotion.PromotionCount > 0)
                                {
                                    // Reduce promotion count by 1
                                    userPromotion.PromotionCount -= 1;
                                    _unitOfWork.UserPromotionRepository.Update(userPromotion);
                                }
                            }
                        }

                        var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(payment.AppointmentId.Value);
                        if (appointment != null && appointment.Status == AppointmentStatus.Pending)
                        {
                            appointment.Status = AppointmentStatus.Confirmed;
                            appointment.IsPaid = true;
                            appointment.UpdatedAt = _timeHelper.NowVietnam();
                            appointment.PaymentId = payment.Id;
                            appointment.UserId = payment.UserId;
                            _unitOfWork.AppointmentRepository.Update(appointment);
                        }
                    }
                    else if (payment.SubscriptionPackageId.HasValue)
                    {
                        if (!string.IsNullOrEmpty(promotionId) && int.TryParse(promotionId, out int promoId) && promoId > 0 &&
                            !string.IsNullOrEmpty(promotionCount) && int.TryParse(promotionCount, out int promoCount) && promoCount > 0)
                        {
                            // Check if user already has this promotion
                            var userPromotions = await _unitOfWork.UserPromotionRepository.GetAllAsync();
                            var existingPromotion = userPromotions.FirstOrDefault(up =>
                                up.UserId.Equals(payment.UserId.Value) &&
                                up.PromotionId == promoId);

                            if (existingPromotion != null)
                            {
                                // Add to existing promotion count
                                existingPromotion.PromotionCount += promoCount;
                                existingPromotion.IsActive = true;
                                _unitOfWork.UserPromotionRepository.Update(existingPromotion);
                            }
                            else
                            {
                                // Create new user promotion
                                var userPromotion = new UserPromotion
                                {
                                    UserId = (Guid)payment.UserId,
                                    PromotionId = promoId,
                                    PromotionCount = promoCount,
                                    IsActive = true,
                                };
                                await _unitOfWork.UserPromotionRepository.AddAsync(userPromotion);
                            }
                        }

                    }

                    await _unitOfWork.CommitAsync();
                }
                else
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
