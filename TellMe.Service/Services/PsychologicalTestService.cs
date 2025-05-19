using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Repository.Infrastructures;
using TellMe.Service.Models.RequestModels;
using TellMe.Service.Models.ResponseModels;
using TellMe.Service.Services.Interface;
using TellMe.Service.Utils;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace TellMe.Service.Services
{
    public class PsychologicalTestService : IPsychologicalTestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITimeHelper _timeHelper;

        public PsychologicalTestService(IUnitOfWork unitOfWork, IMapper mapper, ITimeHelper timeHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _timeHelper = timeHelper;
        }

        public async Task<PsychologicalTestResponse> CreateTestAsync(CreatePsychologicalTestRequest request)
        {
            // Map request to entity
            var testEntity = _mapper.Map<PsychologicalTest>(request);
            testEntity.CreatedAt = _timeHelper.NowVietnam();
            testEntity.UpdatedAt = _timeHelper.NowVietnam();

            // Add to database
            await _unitOfWork.PsychologicalTestRepository.AddAsync(testEntity);
            await _unitOfWork.CommitAsync();

            // Return response
            return _mapper.Map<PsychologicalTestResponse>(testEntity);
        }

        public async Task<PsychologicalTestResponse> UpdateTestAsync(Guid id, UpdatePsychologicalTestRequest request)
        {
            // Tìm bài kiểm tra hiện có với các thực thể liên quan
            var existingTestResult = await _unitOfWork.PsychologicalTestRepository.GetAsync(
                filter: pt => pt.Id == id,
                includeProperties: "Questions.AnswerOptions"
            );

            var existingTest = existingTestResult.Items.FirstOrDefault();

            if (existingTest == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy bài kiểm tra tâm lý với ID {id}");
            }

            // Cập nhật các thuộc tính cơ bản
            existingTest.Title = request.Title;
            existingTest.Description = request.Description;
            existingTest.PsychologicalTestType = request.PsychologicalTestType;
            existingTest.Duration = request.Duration;
            existingTest.UpdatedAt = _timeHelper.NowVietnam(); // Sử dụng UTC để đồng bộ thời gian

            var entityTest = _mapper.Map<PsychologicalTest>(request);
            //// Đồng bộ hóa các câu hỏi
            
            var questionSync = CollectionSyncHelper.SyncCollections(
                existingTest.Questions,
                entityTest.Questions,
                q => q.Id
            );

            // Xử lý các câu hỏi cần thêm
            foreach (var question in questionSync.ToAdd)
            {
                existingTest.Questions.Add(question);
            }

            // Xử lý các câu hỏi cần cập nhật
            foreach (var question in questionSync.ToUpdate)
            {
                var existingQuestion = existingTest.Questions.FirstOrDefault(q => q.Id == question.Id);
                if (existingQuestion != null)
                {
                    // Cập nhật các thuộc tính cơ bản
                    existingQuestion.Content = question.Content;
                    existingQuestion.Order = question.Order;
                    existingQuestion.UpdatedAt = _timeHelper.NowVietnam();

                    // Đồng bộ hóa các tùy chọn trả lời
                    var optionSync = CollectionSyncHelper.SyncCollections(
                        existingQuestion.AnswerOptions,
                        question.AnswerOptions
                            .Select(o => { o.QuestionId = existingQuestion.Id; return o; })
                            .ToList(),
                        o => o.Id
                    );

                    // Xử lý các tùy chọn cần thêm
                    foreach (var option in optionSync.ToAdd)
                    {
                        existingQuestion.AnswerOptions.Add(option);
                    }

                    // Xử lý các tùy chọn cần cập nhật
                    foreach (var option in optionSync.ToUpdate)
                    {
                        var existingOption = existingQuestion.AnswerOptions.FirstOrDefault(o => o.Id == option.Id);
                        if (existingOption != null)
                        {
                            existingOption.Content = option.Content;
                            existingOption.Order = option.Order;
                            existingOption.Score = option.Score;
                            existingOption.UpdatedAt = _timeHelper.NowVietnam();
                        }
                    }

                    // Xử lý các tùy chọn cần xóa (xóa mềm)
                    foreach (var option in optionSync.ToDelete)
                    {
                        var existingOption = existingQuestion.AnswerOptions.FirstOrDefault(o => o.Id == option.Id);
                        if (existingOption != null)
                        {
                            existingOption.IsDeleted = true;
                            existingOption.UpdatedAt = _timeHelper.NowVietnam();
                        }
                    }
                }
            }

            // Xử lý các câu hỏi cần xóa (xóa mềm)
            foreach (var question in questionSync.ToDelete)
            {
                var existingQuestion = existingTest.Questions.FirstOrDefault(q => q.Id == question.Id);
                if (existingQuestion != null)
                {
                    existingQuestion.IsDeleted = true;
                    existingQuestion.UpdatedAt = _timeHelper.NowVietnam();
                    foreach (var option in existingQuestion.AnswerOptions)
                    {
                        option.IsDeleted = true;
                        option.UpdatedAt = _timeHelper.NowVietnam();
                    }
                }
            }



            // Lưu thay đổi vào cơ sở dữ liệu
            await _unitOfWork.CommitAsync();

            var activeQuestions = existingTest.Questions
                .Where(q => !q.IsDeleted)
                .OrderBy(q => q.Order)
                .ToList();

            foreach (var question in activeQuestions)
            {
                question.AnswerOptions = question.AnswerOptions
                    .Where(o => !o.IsDeleted)
                    .OrderBy(o => o.Order)
                    .ToList();
            }

            existingTest.Questions = activeQuestions;
            

            // Trả về kết quả
            return _mapper.Map<PsychologicalTestResponse>(existingTest);
        }

        public async Task<bool> SoftDeleteTestAsync(Guid id)
        {
            // Tìm bài kiểm tra với các thực thể liên quan
            var testResult = await _unitOfWork.PsychologicalTestRepository.GetAsync(
                filter: pt => pt.Id == id,
                includeProperties: "Questions.AnswerOptions"
            );

            var test = testResult.Items.FirstOrDefault();

            if (test == null || test.IsActive == false)
            {
                return false; // Bài kiểm tra không tồn tại hoặc đã bị xóa mềm
            }

            // Đánh dấu bài kiểm tra là không hoạt động (xóa mềm)
            test.IsActive = false;
            test.UpdatedAt = _timeHelper.NowVietnam();

            // Đánh dấu tất cả câu hỏi và tùy chọn trả lời liên quan là đã xóa
            foreach (var question in test.Questions)
            {
                question.IsDeleted = true;
                question.UpdatedAt = _timeHelper.NowVietnam();

                foreach (var option in question.AnswerOptions)
                {
                    option.IsDeleted = true;
                    option.UpdatedAt = _timeHelper.NowVietnam();
                }
            }

            // Cập nhật và lưu thay đổi
            _unitOfWork.PsychologicalTestRepository.Update(test);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<PsychologicalTestResponse> GetTestByIdAsync(Guid id)
        {

            var testResult = await _unitOfWork.PsychologicalTestRepository.GetAsync(
                filter: pt => pt.Id == id && pt.IsActive,
                includeProperties: "Questions.AnswerOptions"
            );

            var test = testResult.Items.FirstOrDefault();

            if (test == null)
            {
                throw new KeyNotFoundException($"Psychological test with ID {id} not found");
            }

            // Filter out deleted questions and answer options
            var activeQuestions = test.Questions.Where(q => !q.IsDeleted).OrderBy(q => q.Order).ToList();

            foreach (var question in activeQuestions)
            {
                question.AnswerOptions = question.AnswerOptions.Where(o => !o.IsDeleted).OrderBy(o => o.Order).ToList();
            }

            test.Questions = activeQuestions;

            return _mapper.Map<PsychologicalTestResponse>(test);
        }

        public async Task<List<PsychologicalTestResponse>> GetAllTestsAsync(bool includeInactive = false)
        {
            var testResult = await _unitOfWork.PsychologicalTestRepository.GetAsync(
                filter: includeInactive ? null : pt => pt.IsActive,
                orderBy: q => q.OrderByDescending(pt => pt.CreatedAt),
                includeProperties: "Questions.AnswerOptions"
            );

            var tests = testResult.Items.ToList();

            // Lọc các câu hỏi và tùy chọn trả lời đã bị xóa
            foreach (var test in tests)
            {
                var activeQuestions = test.Questions
                    .Where(q => !q.IsDeleted)
                    .OrderBy(q => q.Order)
                    .ToList();

                foreach (var question in activeQuestions)
                {
                    question.AnswerOptions = question.AnswerOptions
                        .Where(o => !o.IsDeleted)
                        .OrderBy(o => o.Order)
                        .ToList();
                }

                test.Questions = activeQuestions;
            }

            return _mapper.Map<List<PsychologicalTestResponse>>(tests);
        }
    }
}