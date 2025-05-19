using AutoMapper;
using Azure;
using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Repository.Infrastructures;
using TellMe.Service.Models;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class UserTestService : IUserTestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITimeHelper _timeHelper;

        public UserTestService(IUnitOfWork unitOfWork, IMapper mapper, ITimeHelper timeHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _timeHelper = timeHelper;
        }

        public async Task<UserTestResponse> GetUserTestDetailAsync(Guid userTestId)
        {
            var response = await _unitOfWork.UserTestRepository.GetAsync(
                    filter: ut => ut.Id == userTestId,
                    includeProperties: "Test,UserAnswers.Question,UserAnswers.AnswerOption",
                    pageIndex: 1,
                    pageSize: 1
                );

            var userTest = response.Items.FirstOrDefault();
            return _mapper.Map<UserTestResponse>(userTest);
        }

        public async Task<PaginationObject> GetUserTestHistoryAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            var response = await _unitOfWork.UserTestRepository.GetAsync(
                    filter: ut => ut.UserId == userId,
                    includeProperties: "Test,UserAnswers.Question,UserAnswers.AnswerOption",
                    pageIndex: page,
                    pageSize: pageSize
                );
            var userTestResponses = _mapper.Map<List<UserTestResponse>>(response.Items);
            return new PaginationObject
            {
                PageIndex = page,
                TotalPage = response.TotalPages,
                TotalRecord = response.TotalRecords,
                Data = userTestResponses
            };

        }

        public async Task<UserTestResponse> SubmitTestAnswersAsync(Guid userId, SubmitUserTestRequest request)
        {

            var testResult = await _unitOfWork.PsychologicalTestRepository.GetAsync(
                filter: pt => pt.Id == request.TestId && pt.IsActive,
                includeProperties: "Questions.AnswerOptions"
            );
            var test = testResult.Items.FirstOrDefault();

            int totalScore = 0;
            
            if (test == null)
            {
                throw new KeyNotFoundException($"Active psychological test with ID {request.TestId} not found");
            }

            // Create a new UserTest record
            var userTest = _mapper.Map<UserTest>(request);

            userTest.UserId = userId;

            foreach (var answer in userTest.UserAnswers)
            {
                var question = test.Questions.FirstOrDefault(q => q.Id == answer.QuestionId && !q.IsDeleted);
                if (question == null)
                {
                    continue; // Skip if question not found or deleted
                }

                var selectedOption = question.AnswerOptions.FirstOrDefault(o => o.Id == answer.AnswerOptionId && !o.IsDeleted);
                if (selectedOption == null)
                {
                    continue; // Skip if answer option not found or deleted
                }

                answer.Score = selectedOption.Score;
                totalScore += selectedOption.Score;

                // Add to question results
                answer.Question = question;
                answer.AnswerOption = selectedOption;
            }

            // Update UserTest with final score
            userTest.TotalScore = totalScore;
            userTest.CreatedAt = _timeHelper.NowVietnam();

            await _unitOfWork.UserTestRepository.AddAsync(userTest);

            // Save all changes
            await _unitOfWork.CommitAsync();

            return _mapper.Map<UserTestResponse>(userTest);
        }
    }
}
