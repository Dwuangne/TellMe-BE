using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;

namespace TellMe.Service.Services.Interface
{
    public interface IPatientProfileService
    {
        public Task<List<PatientProfile>> GetAllPatientProfilesAsync(Guid? userId = null);
        public Task<List<PatientProfile>> GetAllActivePatientProfilesAsync(Guid? userId = null);
        public Task<List<PatientProfile>> GetAllActivePatientProfilesAsyncForExpert(Guid? expertId = null);
        public Task<PatientProfile> GetPatientProfileAsync(Guid userId);
        public Task<PatientProfile> CreatePatientProfileAsync(CreatePatientProfileRequest patientProfile);
        public Task<PatientProfile> UpdatePatientProfileAsync(int id, UpdatePatientProfileRequest patientProfile);
        public bool ManageDeletePatientProfile(int id, bool isActive = false);
    }
}
