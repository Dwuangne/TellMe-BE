using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITimeHelper _timeHelper;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, ITimeHelper timeHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _timeHelper = timeHelper;
        }

        public async Task<AppointmentResponse> CreateAppointmentAsync(Guid expertId, CreateAppointmentRequest request)
        {
            // Validate expert exists
            var expert = await _userManager.FindByIdAsync(expertId.ToString());
            if (expert == null)
                throw new ArgumentException("Expert not found");

            // Validate appointment time is in future
            if (request.AppointmentDateTime <= _timeHelper.NowVietnam())
                throw new ArgumentException("Appointment time must be in the future");

            // Check for conflicting appointments
            var conflicting = await _unitOfWork.AppointmentRepository.FirstOrDefaultAsync(
                a => a.ExpertId == expertId &&
                     a.IsActive &&
                     a.Status != AppointmentStatus.Cancelled &&
                     a.AppointmentDateTime < request.AppointmentDateTime.AddMinutes(request.DurationMinutes) &&
                     request.AppointmentDateTime < a.AppointmentDateTime.AddMinutes(a.DurationMinutes)
            );

            if (conflicting != null)
                throw new InvalidOperationException("Time slot is not available");

            var appointment = new Appointment
            {
                ExpertId = expertId,
                AppointmentDateTime = _timeHelper.NormalizeToVietnam(request.AppointmentDateTime),
                DurationMinutes = request.DurationMinutes,
                Notes = request.Notes,
                Fee = request.Fee,
                MeetingURL = request.MeetingURL,
                Status = AppointmentStatus.Pending,
                IsActive = true,
                CreatedAt = _timeHelper.NowVietnam(),
                UpdatedAt = _timeHelper.NowVietnam(),
            };

            await _unitOfWork.AppointmentRepository.AddAsync(appointment);
            await _unitOfWork.CommitAsync();

            return await MapToResponseAsync(appointment);
        }

        public async Task<bool> CancelAppointmentAsync(Guid id)
        {
            var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null || !appointment.IsActive)
                return false;

            if (appointment.Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Cannot cancel completed appointment");

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.IsActive = false;
            appointment.UpdatedAt = DateTime.Now;

            _unitOfWork.AppointmentRepository.Update(appointment);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> ConfirmPaymentAsync(Guid id, Guid paymentId)
        {
            var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null || !appointment.IsActive)
                return false;

            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
            if (payment == null || payment.Status != PaymentStatus.Success)
                throw new InvalidOperationException("Invalid or unsuccessful payment");

            appointment.PaymentId = paymentId;
            appointment.IsPaid = true;
            appointment.Status = AppointmentStatus.Confirmed;
            appointment.UpdatedAt = _timeHelper.NowVietnam();

            _unitOfWork.AppointmentRepository.Update(appointment);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<IEnumerable<AppointmentResponse>> GetAllAppointmentsAsync(Guid? userId = null, Guid? expertId = null)
        {
            var appointments = await _unitOfWork.AppointmentRepository.GetAsync(
                filter: a => (userId == null || a.UserId == userId) &&
                            (expertId == null || a.ExpertId == expertId),
                orderBy: q => q.OrderByDescending(a => a.AppointmentDateTime)
                //includeProperties: "Payment"
            );

            var responses = new List<AppointmentResponse>();
            foreach (var appointment in appointments.Items)
            {
                responses.Add(await MapToResponseAsync(appointment));
            }

            return responses;
        }

        public async Task<IEnumerable<AppointmentResponse>> GetActiveAppointmentsAsync(Guid? userId = null, Guid? expertId = null)
        {
            var appointments = await _unitOfWork.AppointmentRepository.GetAsync(
                filter: a => (userId == null || a.UserId == userId) &&
                            (expertId == null || a.ExpertId == expertId) &&
                            a.IsActive,
                orderBy: q => q.OrderByDescending(a => a.AppointmentDateTime)
                //includeProperties: "Payment"
            );

            var responses = new List<AppointmentResponse>();
            foreach (var appointment in appointments.Items)
            {
                responses.Add(await MapToResponseAsync(appointment));
            }

            return responses;
        }

        public async Task<AppointmentResponse?> GetAppointmentByIdAsync(Guid id)
        {
            var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
            return appointment == null ? null : await MapToResponseAsync(appointment);
        }

        public async Task<AppointmentResponse> UpdateAppointmentAsync(Guid id, UpdateAppointmentRequest request)
        {
            var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null || !appointment.IsActive)
                throw new KeyNotFoundException("Appointment not found");

            if (appointment.Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Cannot update completed appointment");

            // Update only provided fields
            if (request.AppointmentDateTime.HasValue)
            {
                if (request.AppointmentDateTime.Value <= _timeHelper.NowVietnam())
                    throw new ArgumentException("Appointment time must be in the future");
                
                appointment.AppointmentDateTime = _timeHelper.NormalizeToVietnam(request.AppointmentDateTime.Value);
            }

            if (request.DurationMinutes.HasValue)
                appointment.DurationMinutes = request.DurationMinutes.Value;

            if (request.Notes != null)
                appointment.Notes = request.Notes;

            if (request.Status.HasValue)
                appointment.Status = request.Status.Value;

            appointment.Fee = request.Fee;

            if (request.MeetingURL != null)
                appointment.MeetingURL = request.MeetingURL;

            appointment.UpdatedAt = _timeHelper.NowVietnam();

            _unitOfWork.AppointmentRepository.Update(appointment);
            await _unitOfWork.CommitAsync();

            return await MapToResponseAsync(appointment);
        }

        public async Task<AppointmentResponse> UpdateAppointmentStatusAsync(Guid id, AppointmentStatus status)
        {
            var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null || !appointment.IsActive)
                throw new KeyNotFoundException("Appointment not found");

            appointment.Status = status;
            appointment.UpdatedAt = _timeHelper.NowVietnam();

            _unitOfWork.AppointmentRepository.Update(appointment);
            await _unitOfWork.CommitAsync();

            return await MapToResponseAsync(appointment);
        }

        private async Task<AppointmentResponse> MapToResponseAsync(Appointment appointment)
        {
            var response = _mapper.Map<AppointmentResponse>(appointment);

            // Get expert name
            var expert = await _userManager.FindByIdAsync(appointment.ExpertId.ToString());
            if (expert != null)
            {
                response.ExpertName = expert.FullName;
            }

            // Map payment if exists
            if (appointment.PaymentId.HasValue)
            {
                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(appointment.PaymentId.Value);
                if (payment != null)
                {
                    response.Payment = _mapper.Map<PaymentResponse>(payment);
                }
            }

            return response;
        }
    }
}
