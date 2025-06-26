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
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class PsychologicalAssessmentService : IPsychologicalAssessmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITimeHelper _timeHelper;

        public PsychologicalAssessmentService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, ITimeHelper timeHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _timeHelper = timeHelper;
        }

        public async Task<PsychologicalAssessment> CreatePsychologicalAssessmentAsync(CreatePsychologicalAssessmentRequest request)
        {
            var assessment = _mapper.Map<PsychologicalAssessment>(request);
            assessment.AssessmentDate = _timeHelper.NowVietnam();
            assessment.EditDate = _timeHelper.NowVietnam();
            assessment.IsActive = true;

            await _unitOfWork.PsychologicalAssessmentRepository.AddAsync(assessment);
            await _unitOfWork.CommitAsync();

            return assessment;
        }

        public async Task<List<PsychologicalAssessment>> GetAllActivePsychologicalAssessmentsAsync(Guid? userId = null, Guid? expertId = null)
        {
            var assessments = await _unitOfWork.PsychologicalAssessmentRepository.GetAsync(
                filter: a => a.IsActive &&
                             (!userId.HasValue || a.UserId == userId.Value) &&
                             (!expertId.HasValue || a.ExpertId == expertId.Value),
                orderBy: q => q.OrderByDescending(a => a.AssessmentDate)
            );

            return assessments.Items.ToList();
        }

        public async Task<List<PsychologicalAssessment>> GetAllPsychologicalAssessmentsAsync(Guid? userId = null, Guid? expertId = null)
        {
            var assessments = await _unitOfWork.PsychologicalAssessmentRepository.GetAsync(
                filter: a => (userId == null || a.UserId == userId.Value) &&
                             (expertId == null || a.ExpertId == expertId.Value),
                orderBy: q => q.OrderByDescending(a => a.AssessmentDate)
            );

            return assessments.Items.ToList();
        }

        public async Task<PsychologicalAssessment> GetPsychologicalAssessmentAsync(int assessmentId)
        {
            var assessment = await _unitOfWork.PsychologicalAssessmentRepository.GetByIdAsync(assessmentId);

            if (assessment == null)
            {
                return null;
            }

            return assessment;
        }

        public bool ManageDeletePsychologicalAssessment(int id, bool isActive = false)
        {
            try
            {
                var assessment = _unitOfWork.PsychologicalAssessmentRepository.GetByIdAsync(id).Result;

                if (assessment == null)
                {
                    return false;
                }

                assessment.IsActive = isActive;
                _unitOfWork.PsychologicalAssessmentRepository.Update(assessment);
                _unitOfWork.Commit();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<PsychologicalAssessment> UpdatePsychologicalAssessmentAsync(int id, UpdatePsychologicalAssessmentRequest request)
        {
            var assessment = await _unitOfWork.PsychologicalAssessmentRepository.GetByIdAsync(id);

            if (assessment == null)
            {
                return null;
            }

            _mapper.Map(request, assessment);
            assessment.EditDate = _timeHelper.NowVietnam();

            _unitOfWork.PsychologicalAssessmentRepository.Update(assessment);
            await _unitOfWork.CommitAsync();

            return assessment;
        }
    }
}
