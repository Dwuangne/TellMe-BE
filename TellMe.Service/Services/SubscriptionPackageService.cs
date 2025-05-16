using AutoMapper;
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
    public class SubscriptionPackageService : ISubscriptionPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubscriptionPackageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SubscriptionPackageResponse> CreatePackageAsync(CreatePackageRequest request)
        {
            // Check if package name already exists
            var existingPackage = await _unitOfWork.SubscriptionPackageRepository
                .FirstOrDefaultAsync(p => p.PackageName == request.PackageName && p.IsActive);
            
            if (existingPackage != null)
                throw new InvalidOperationException("A package with this name already exists");

            var package = _mapper.Map<SubscriptionPackage>(request);
            package.IsActive = true;

            await _unitOfWork.SubscriptionPackageRepository.AddAsync(package);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<SubscriptionPackageResponse>(package);
        }

        public async Task<bool> DeletePackageAsync(int id)
        {
            var package = await _unitOfWork.SubscriptionPackageRepository.GetByIdAsync(id);
            if (package == null || !package.IsActive)
                return false;

            // Check if package is being used by any active subscriptions
            var activeSubscriptions = await _unitOfWork.UserSubscriptionRepository
                .FirstOrDefaultAsync(us => us.PackageId == id && us.IsActive);
            
            if (activeSubscriptions != null)
                throw new InvalidOperationException("Cannot delete package with active subscriptions");

            package.IsActive = false;
            package.LastModifiedDate = DateTime.Now;

            _unitOfWork.SubscriptionPackageRepository.Update(package);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<IEnumerable<SubscriptionPackageResponse>> GetAllPackagesActiveAsync(bool includeInactive = false)
        {
            var packages = await _unitOfWork.SubscriptionPackageRepository.GetAsync(
                filter: p => includeInactive || p.IsActive,
                orderBy: q => q.OrderBy(p => p.Duration)
            );

            return _mapper.Map<IEnumerable<SubscriptionPackageResponse>>(packages.Items);
        }

        public async Task<IEnumerable<SubscriptionPackageResponse>> GetAllPackagesAsync(bool includeInactive = true)
        {
            var packages = await _unitOfWork.SubscriptionPackageRepository.GetAsync(
                filter: includeInactive ? null : p => p.IsActive,
                orderBy: q => q.OrderBy(p => p.Duration)
            );

            return _mapper.Map<IEnumerable<SubscriptionPackageResponse>>(packages.Items);
        }

        public async Task<SubscriptionPackageResponse?> GetPackageByIdAsync(int id)
        {
            var package = await _unitOfWork.SubscriptionPackageRepository.GetByIdAsync(id);
            if (package == null)
                return null;

            return _mapper.Map<SubscriptionPackageResponse>(package);
        }

        public async Task<bool> RestorePackageAsync(int id)
        {
            var package = await _unitOfWork.SubscriptionPackageRepository.GetByIdAsync(id);
            if (package == null || package.IsActive)
                return false;

            // Check if another active package exists with the same name
            var existingPackage = await _unitOfWork.SubscriptionPackageRepository
                .FirstOrDefaultAsync(p => p.PackageName == package.PackageName && p.IsActive);
            
            if (existingPackage != null)
                throw new InvalidOperationException("An active package with this name already exists");

            package.IsActive = true;
            package.LastModifiedDate = DateTime.Now;

            _unitOfWork.SubscriptionPackageRepository.Update(package);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<SubscriptionPackageResponse> UpdatePackageAsync(int id, UpdatePackageRequest request)
        {
            var package = await _unitOfWork.SubscriptionPackageRepository.GetByIdAsync(id);
            if (package == null)
                throw new KeyNotFoundException($"Package with ID {id} not found");

            // Check if new package name conflicts with existing packages
            if (package.PackageName != request.PackageName)
            {
                var existingPackage = await _unitOfWork.SubscriptionPackageRepository
                    .FirstOrDefaultAsync(p => p.PackageName == request.PackageName && p.IsActive && p.Id != id);
                
                if (existingPackage != null)
                    throw new InvalidOperationException("A package with this name already exists");
            }

            // Update package properties
            _mapper.Map(request, package);
            package.LastModifiedDate = DateTime.Now;

            // If deactivating, check for active subscriptions
            //if (!request.IsActive && package.IsActive)
            //{
            //    var activeSubscriptions = await _unitOfWork.UserSubscriptionRepository
            //        .FirstOrDefaultAsync(us => us.PackageId == id && us.IsActive);
                
            //    if (activeSubscriptions != null)
            //        throw new InvalidOperationException("Cannot deactivate package with active subscriptions");
            //}

            _unitOfWork.SubscriptionPackageRepository.Update(package);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<SubscriptionPackageResponse>(package);
        }
    }
}
