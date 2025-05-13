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


            CreateMap<AnswerOption, AnswerOptionResponse>().ReverseMap();
            CreateMap<PsychologicalTest, PsychologicalTestResponse>().ReverseMap();
            CreateMap<Question, QuestionResponse>().ReverseMap();
            CreateMap<UserTest, UserTestReponse>().ReverseMap();
            CreateMap<UserAnswer, UserAnswerResponse>().ReverseMap();

        }
    }
}
