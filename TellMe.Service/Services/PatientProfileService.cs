using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Repository.Infrastructures;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class PatientProfileService : IPatientProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITimeHelper _timeHelper;

        public PatientProfileService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, ITimeHelper timeHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _timeHelper = timeHelper;
        }

        public async Task<PatientProfile> CreatePatientProfileAsync(CreatePatientProfileRequest request)
        {
            var patientProfile = _mapper.Map<PatientProfile>(request);
            patientProfile.IsActive = true;

            await _unitOfWork.PatientProfileRepository.AddAsync(patientProfile);
            await _unitOfWork.CommitAsync();

            return patientProfile;
        }

        public async Task<List<PatientProfile>> GetAllActivePatientProfilesAsync(Guid? userId = null)
        {
            var profiles = await _unitOfWork.PatientProfileRepository.GetAsync(
                filter: p => p.IsActive &&
                            (userId == null || p.UserId == userId.Value));

            var response = new List<PatientProfile>();
            foreach (var profile in profiles.Items)
            {
                response.Add(profile);
            }

            return response;
        }

        public async Task<List<PatientProfile>> GetAllPatientProfilesAsync(Guid? userId = null)
        {
            var profiles = await _unitOfWork.PatientProfileRepository.GetAsync(
                filter: p => userId == null || p.UserId == userId.Value);

            var response = new List<PatientProfile>();
            foreach (var profile in profiles.Items)
            {
                response.Add(profile);
            }

            return response;
        }

        public async Task<PatientProfile> GetPatientProfileAsync(Guid userId)
        {
            var profile = await _unitOfWork.PatientProfileRepository.FirstOrDefaultAsync(
                p => p.UserId == userId && p.IsActive);

            if (profile == null)
            {
                throw new KeyNotFoundException($"Patient profile with User ID {userId} not found.");
            }

            //var response = _mapper.Map<PatientProfileResponse>(profile);

            //var assessments = await _unitOfWork.PsychologicalAssessmentRepository.GetAsync(
            //    filter: a => a.UserId == userId && a.IsActive,
            //    orderBy: q => q.OrderByDescending(a => a.AssessmentDate)
            //);

            //response.Assessments = assessments.Items.ToList();

            return profile;
        }

        public bool ManageDeletePatientProfile(int id, bool isActive = false)
        {
            try
            {
                var profile = _unitOfWork.PatientProfileRepository.GetByIdAsync(id).Result;

                if (profile == null)
                {
                    return false;
                }

                profile.IsActive = isActive;
                _unitOfWork.PatientProfileRepository.Update(profile);
                _unitOfWork.Commit();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<PatientProfile> UpdatePatientProfileAsync(int id, UpdatePatientProfileRequest request)
        {
            var existingProfile = await _unitOfWork.PatientProfileRepository.GetByIdAsync(id);

            if (existingProfile == null)
            {
                throw new KeyNotFoundException($"Patient profile with ID {id} not found.");
            }

            _mapper.Map(request, existingProfile);

            _unitOfWork.PatientProfileRepository.Update(existingProfile);
            await _unitOfWork.CommitAsync();

            return existingProfile;
        }
    }
}
