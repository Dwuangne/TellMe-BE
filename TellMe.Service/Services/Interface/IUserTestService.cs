using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;

namespace TellMe.Service.Services.Interface
{
    public interface IUserTestService
    {
        Task<UserTestReponse> SubmitTestAnswersAsync(Guid userId, SubmitUserTestRequest request);
        Task<PaginationObject> GetUserTestHistoryAsync(Guid userId, int page = 1, int pageSize = 10);
    }
}
