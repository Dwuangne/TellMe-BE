using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;

namespace TellMe.Service.Services.Interface
{
    public interface ISubscriptionPackageService
    {
        Task<IEnumerable<SubscriptionPackageResponse>> GetAllPackagesAsync(bool includeInactive = false);
        Task<IEnumerable<SubscriptionPackageResponse>> GetAllPackagesActiveAsync(bool includeInactive = false);
        Task<SubscriptionPackageResponse?> GetPackageByIdAsync(int id);
        Task<SubscriptionPackageResponse> CreatePackageAsync(CreatePackageRequest request);
        Task<SubscriptionPackageResponse> UpdatePackageAsync(int id, UpdatePackageRequest request);
        Task<bool> DeletePackageAsync(int id);
        Task<bool> RestorePackageAsync(int id);
    }
}
