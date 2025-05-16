using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;

namespace TellMe.Service.Services.Interface
{
    public interface IPsychologicalService
    {
        Task<PsychologistResponse> CreatePsychologistAsync(PsychologistCreateRequest request);
        Task<PsychologistResponse> UpdatePsychologistAsync(Guid id, PsychologistUpdateRequest request);
        Task<PsychologistResponse> GetPsychologistByIdAsync(Guid id);
        Task<List<PsychologistResponse>> GetAllPsychologistsAsync();
        Task<bool> DeletePsychologistAsync(Guid id);
    }
}
