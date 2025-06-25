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
using TellMe.Service.Utils;

namespace TellMe.Service.Services
{
    public class PsychologicalService : IPsychologicalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITimeHelper _timeHelper;

        public PsychologicalService(IUnitOfWork unitOfWork, IMapper mapper, ITimeHelper timeHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _timeHelper = timeHelper;
        }

        public async Task<PsychologistResponse> CreatePsychologistAsync(PsychologistCreateRequest request)
        {
            // Map request to entity
            var psychologistEntity = _mapper.Map<Psychologist>(request);

            // Set default values
            psychologistEntity.IsVerified = false;
            psychologistEntity.CreatedAt = _timeHelper.NowVietnam();
            psychologistEntity.UpdatedAt = _timeHelper.NowVietnam();

            // Add to database
            await _unitOfWork.PsychologistRepository.AddAsync(psychologistEntity);
            await _unitOfWork.CommitAsync();

            // Return response
            return _mapper.Map<PsychologistResponse>(psychologistEntity);
        }

        public async Task<bool> DeletePsychologistAsync(Guid id)
        {
            // Find the psychologist with related entities
            var psychologistResult = await _unitOfWork.PsychologistRepository.GetAsync(
                filter: p => p.Id == id,
                includeProperties: "Educations,Experiences,Licenses"
            );

            var psychologist = psychologistResult.Items.FirstOrDefault();

            if (psychologist == null)
            {
                return false; // Psychologist doesn't exist
            }

            // Mark educations as deleted
            foreach (var education in psychologist.Educations)
            {
                education.IsDeleted = true;
                education.UpdatedAt = _timeHelper.NowVietnam();
            }

            // Mark experiences as deleted
            foreach (var experience in psychologist.Experiences)
            {
                experience.IsDeleted = true;
                experience.UpdatedAt = _timeHelper.NowVietnam();
            }

            // Mark licenses as deleted
            foreach (var license in psychologist.Licenses)
            {
                license.IsDeleted = true;
                license.UpdatedAt = _timeHelper.NowVietnam();
            }

            // Update the entity (soft delete could be implemented if IsDeleted exists on Psychologist)
            psychologist.UpdatedAt = _timeHelper.NowVietnam();
            _unitOfWork.PsychologistRepository.Update(psychologist);

            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<List<PsychologistResponse>> GetAllPsychologistsAsync()
        {
            var psychologistResult = await _unitOfWork.PsychologistRepository.GetAsync(
                orderBy: q => q.OrderByDescending(p => p.CreatedAt),
                includeProperties: "Educations,Experiences,Licenses"
            );

            var psychologists = psychologistResult.Items.ToList();

            // Filter out deleted related entities
            foreach (var psychologist in psychologists)
            {
                psychologist.Educations = psychologist.Educations
                    .Where(e => !e.IsDeleted)
                    .ToList();

                psychologist.Experiences = psychologist.Experiences
                    .Where(e => !e.IsDeleted)
                    .ToList();

                psychologist.Licenses = psychologist.Licenses
                    .Where(l => !l.IsDeleted)
                    .ToList();
            }

            return _mapper.Map<List<PsychologistResponse>>(psychologists);
        }

        public async Task<PsychologistResponse> GetPsychologistByIdAsync(Guid id)
        {
            var psychologistResult = await _unitOfWork.PsychologistRepository.GetAsync(
                filter: p => p.Id == id,
                includeProperties: "Educations,Experiences,Licenses"
            );

            var psychologist = psychologistResult.Items.FirstOrDefault();

            if (psychologist == null)
            {
                throw new KeyNotFoundException($"Psychologist with ID {id} not found");
            }

            // Filter out deleted related entities
            psychologist.Educations = psychologist.Educations
                .Where(e => !e.IsDeleted)
                .ToList();

            psychologist.Experiences = psychologist.Experiences
                .Where(e => !e.IsDeleted)
                .ToList();

            psychologist.Licenses = psychologist.Licenses
                .Where(l => !l.IsDeleted)
                .ToList();

            return _mapper.Map<PsychologistResponse>(psychologist);
        }

        public async Task<PsychologistResponse> UpdatePsychologistAsync(Guid id, PsychologistUpdateRequest request)
        {
            // Find existing psychologist with related entities
            var psychologistResult = await _unitOfWork.PsychologistRepository.GetAsync(
                filter: p => p.Id == id,
                includeProperties: "Educations,Experiences,Licenses"
            );

            var existingPsychologist = psychologistResult.Items.FirstOrDefault();

            if (existingPsychologist == null)
            {
                throw new KeyNotFoundException($"Psychologist with ID {id} not found");
            }

            // Update basic properties
            existingPsychologist.Name = request.Name;
            existingPsychologist.Specialization = request.Specialization;
            existingPsychologist.Address = request.Address;
            existingPsychologist.DateOfBirth = request.DateOfBirth;
            existingPsychologist.Bio = request.Bio;
            existingPsychologist.AvatarUrl = request.AvatarUrl;
            existingPsychologist.UpdatedAt = _timeHelper.NowVietnam();

            // Create entity from request for syncing collections
            var entityPsychologist = new Psychologist
            {
                Id = id,
                Educations = _mapper.Map<List<PsychologistEducation>>(request.Educations),
                Experiences = _mapper.Map<List<PsychologistExperience>>(request.Experiences),
                Licenses = _mapper.Map<List<PsychologistLicenseCertification>>(request.Licenses)
            };

            // Sync Educations
            var educationSync = CollectionSyncHelper.SyncCollections(
                existingPsychologist.Educations,
                entityPsychologist.Educations,
                e => e.Id
            );

            // Handle educations to add
            foreach (var education in educationSync.ToAdd)
            {
                education.PsychologistId = id;
                existingPsychologist.Educations.Add(education);
            }

            // Handle educations to update
            foreach (var education in educationSync.ToUpdate)
            {
                var existingEducation = existingPsychologist.Educations.FirstOrDefault(e => e.Id == education.Id);
                if (existingEducation != null)
                {
                    existingEducation.Degree = education.Degree;
                    existingEducation.Institution = education.Institution;
                    existingEducation.StartDate = education.StartDate;
                    existingEducation.EndDate = education.EndDate;
                    existingEducation.Description = education.Description;
                    existingEducation.CertificateFile = education.CertificateFile;
                    existingEducation.IsDeleted = education.IsDeleted; // Add this line to handle deletion via update
                    existingEducation.UpdatedAt = _timeHelper.NowVietnam();
                }
            }

            // Handle educations to delete
            foreach (var education in educationSync.ToDelete)
            {
                var existingEducation = existingPsychologist.Educations.FirstOrDefault(e => e.Id == education.Id);
                if (existingEducation != null)
                {
                    existingEducation.IsDeleted = true;
                    existingEducation.UpdatedAt = _timeHelper.NowVietnam();
                }
            }

            // Sync Experiences
            var experienceSync = CollectionSyncHelper.SyncCollections(
                existingPsychologist.Experiences,
                entityPsychologist.Experiences,
                e => e.Id
            );

            // Handle experiences to add
            foreach (var experience in experienceSync.ToAdd)
            {
                experience.PsychologistId = id;
                existingPsychologist.Experiences.Add(experience);
            }

            // Handle experiences to update
            foreach (var experience in experienceSync.ToUpdate)
            {
                var existingExperience = existingPsychologist.Experiences.FirstOrDefault(e => e.Id == experience.Id);
                if (existingExperience != null)
                {
                    existingExperience.Position = experience.Position;
                    existingExperience.Institution = experience.Institution;
                    existingExperience.StartDate = experience.StartDate;
                    existingExperience.EndDate = experience.EndDate;
                    existingExperience.IsCurrent = experience.IsCurrent;
                    existingExperience.Description = experience.Description;
                    existingExperience.IsDeleted = experience.IsDeleted; // Add this line to handle deletion via update
                    existingExperience.UpdatedAt = _timeHelper.NowVietnam();
                }
            }

            // Handle experiences to delete
            foreach (var experience in experienceSync.ToDelete)
            {
                var existingExperience = existingPsychologist.Experiences.FirstOrDefault(e => e.Id == experience.Id);
                if (existingExperience != null)
                {
                    existingExperience.IsDeleted = true;
                    existingExperience.UpdatedAt = _timeHelper.NowVietnam();
                }
            }

            // Sync Licenses
            var licenseSync = CollectionSyncHelper.SyncCollections(
                existingPsychologist.Licenses,
                entityPsychologist.Licenses,
                l => l.Id
            );

            // Handle licenses to add
            foreach (var license in licenseSync.ToAdd)
            {
                license.PsychologistId = id;
                existingPsychologist.Licenses.Add(license);
            }

            // Handle licenses to update
            foreach (var license in licenseSync.ToUpdate)
            {
                var existingLicense = existingPsychologist.Licenses.FirstOrDefault(l => l.Id == license.Id);
                if (existingLicense != null)
                {
                    existingLicense.Name = license.Name;
                    existingLicense.LicenseNumber = license.LicenseNumber;
                    existingLicense.IssuingAuthority = license.IssuingAuthority;
                    existingLicense.IssueDate = license.IssueDate;
                    existingLicense.ExpiryDate = license.ExpiryDate;
                    existingLicense.DocumentPath = license.DocumentPath;
                    existingLicense.IsDeleted = license.IsDeleted; // Add this line to handle deletion via update
                    existingLicense.UpdatedAt = _timeHelper.NowVietnam();
                }
            }

            // Handle licenses to delete
            foreach (var license in licenseSync.ToDelete)
            {
                var existingLicense = existingPsychologist.Licenses.FirstOrDefault(l => l.Id == license.Id);
                if (existingLicense != null)
                {
                    existingLicense.IsDeleted = true;
                    existingLicense.UpdatedAt = _timeHelper.NowVietnam();
                }
            }

            // Save changes to database
            await _unitOfWork.CommitAsync();

            // Filter out deleted related entities for response
            existingPsychologist.Educations = existingPsychologist.Educations
                .Where(e => !e.IsDeleted)
                .ToList();

            existingPsychologist.Experiences = existingPsychologist.Experiences
                .Where(e => !e.IsDeleted)
                .ToList();

            existingPsychologist.Licenses = existingPsychologist.Licenses
                .Where(l => !l.IsDeleted)
                .ToList();

            // Return response
            return _mapper.Map<PsychologistResponse>(existingPsychologist);
        }
    }

}
