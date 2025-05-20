using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;

namespace TellMe.Service.Mapping
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AnswerOptionRequest, AnswerOption>().ReverseMap();
            CreateMap<QuestionRequest, Question>().ReverseMap();
            CreateMap<CreatePsychologicalTestRequest, PsychologicalTest>().ReverseMap();
            CreateMap<UpdatePsychologicalTestRequest, PsychologicalTest>().ReverseMap();
            CreateMap<SubmitUserTestRequest, UserTest>().ReverseMap();
            CreateMap<UserAnswerSubmission, UserAnswer>().ReverseMap();
            CreateMap<SubscriptionPackage, CreatePackageRequest>().ReverseMap();
            CreateMap<SubscriptionPackage, UpdatePackageRequest>().ReverseMap();
            CreateMap<Appointment, CreateAppointmentRequest>().ReverseMap();
            CreateMap<Appointment, UpdateAppointmentRequest>().ReverseMap();

            CreateMap<AnswerOption, AnswerOptionResponse>().ReverseMap();
            CreateMap<PsychologicalTest, PsychologicalTestResponse>().ReverseMap();
            CreateMap<Question, QuestionResponse>().ReverseMap();
            CreateMap<UserTest, UserTestResponse>().ReverseMap();
            CreateMap<UserAnswer, UserAnswerResponse>().ReverseMap();
            CreateMap<PsychologistCreateRequest, Psychologist>().ReverseMap();
            CreateMap<PsychologistUpdateRequest, Psychologist>().ReverseMap();
            CreateMap<Psychologist, PsychologistResponse>().ReverseMap();
            CreateMap<PsychologistEducationRequest, PsychologistEducation>().ReverseMap();
            CreateMap<PsychologistEducation, PsychologistEducationResponse>().ReverseMap();
            CreateMap<PsychologistExperienceRequest, PsychologistExperience>().ReverseMap();
            CreateMap<PsychologistExperience, PsychologistExperienceResponse>().ReverseMap(); 
            CreateMap<PsychologistLicenseCertificationRequest, PsychologistLicenseCertification>().ReverseMap();
            CreateMap<PsychologistLicenseCertification, PsychologistLicenseCertificationResponse>().ReverseMap(); 
            CreateMap<PsychologistReview, PsychologistReviewResponse>().ReverseMap();
            CreateMap<SubscriptionPackage, SubscriptionPackageResponse>().ReverseMap();
            CreateMap<Payment, PaymentResponse>().ReverseMap();
            CreateMap<UserSubscription, UserSubscriptionResponse>().ReverseMap();
            CreateMap<Appointment, AppointmentResponse>().ReverseMap();
            CreateMap<ApplicationUser, ProfileResponse>().ReverseMap();

        }
    }
}
