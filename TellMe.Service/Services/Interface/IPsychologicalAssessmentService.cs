using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Service.Models.RequestModels;

namespace TellMe.Service.Services.Interface
{
    public interface IPsychologicalAssessmentService
    {
        public Task<List<PsychologicalAssessment>> GetAllPsychologicalAssessmentsAsync(Guid? userId = null, Guid? expertId = null);
        public Task<List<PsychologicalAssessment>> GetAllActivePsychologicalAssessmentsAsync(Guid? userId = null, Guid? expertId = null);
        public Task<PsychologicalAssessment> GetPsychologicalAssessmentAsync(int assessmentId);
        public Task<PsychologicalAssessment> CreatePsychologicalAssessmentAsync(CreatePsychologicalAssessmentRequest psychologicalAssessment);
        public Task<PsychologicalAssessment> UpdatePsychologicalAssessmentAsync(int id, UpdatePsychologicalAssessmentRequest psychologicalAssessment);
        public bool ManageDeletePsychologicalAssessment(int id, bool isActive = false);
    }
}
