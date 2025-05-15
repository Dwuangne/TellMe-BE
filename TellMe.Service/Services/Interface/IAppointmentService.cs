using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enums;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;

namespace TellMe.Service.Services.Interface
{
    public interface IAppointmentService
    {
        Task<AppointmentResponse> CreateAppointmentAsync(Guid expertId, CreateAppointmentRequest request);
        Task<AppointmentResponse?> GetAppointmentByIdAsync(Guid id);
        Task<IEnumerable<AppointmentResponse>> GetAllAppointmentsAsync(Guid? userId = null, Guid? expertId = null);
        Task<IEnumerable<AppointmentResponse>> GetActiveAppointmentsAsync(Guid? userId = null, Guid? expertId = null);
        Task<AppointmentResponse> UpdateAppointmentAsync(Guid id, UpdateAppointmentRequest request);
        Task<bool> CancelAppointmentAsync(Guid id);
        Task<AppointmentResponse> UpdateAppointmentStatusAsync(Guid id, AppointmentStatus status);
        Task<bool> ConfirmPaymentAsync(Guid id, Guid paymentId);
    }
}
