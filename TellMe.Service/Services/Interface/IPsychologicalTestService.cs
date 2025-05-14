using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;

namespace TellMe.Service.Services.Interface
{
    public interface IPsychologicalTestService
    {
        Task<PsychologicalTestResponse> CreateTestAsync(CreatePsychologicalTestRequest request);
        Task<PsychologicalTestResponse> UpdateTestAsync(Guid id, UpdatePsychologicalTestRequest request);
        Task<bool> SoftDeleteTestAsync(Guid id);
        Task<PsychologicalTestResponse> GetTestByIdAsync(Guid id);
        Task<List<PsychologicalTestResponse>> GetAllTestsAsync(bool includeInactive = false);
    }
} 