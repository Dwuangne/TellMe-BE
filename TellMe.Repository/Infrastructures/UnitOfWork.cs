﻿using Microsoft.Extensions.Configuration;
using System;
using System.Numerics;
using System.Threading.Tasks;
using TellMe.Repository.DBContexts;
using TellMe.Repository.Enities;
using TellMe.Repository.Repositories;
using TellMe.Repository.SMTPs.Repositories;

namespace TellMe.Repository.Infrastructures
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TellMeDBContext _dbContext;
        private readonly TellMeAuthDBContext _dbAuthContext;

        private IGenericRepository<Psychologist> _psychologistRepository;
        private IGenericRepository<PsychologistEducation> _psychologistEducationRepository;
        private IGenericRepository<PsychologistExperience> _psychologistExperienceRepository;
        private IGenericRepository<PsychologistLicenseCertification> _psychologistLicenseCertificationRepository;
        private IGenericRepository<PsychologicalTest> _psychologicalTestRepository;
        private IGenericRepository<Question> _questionRepository;
        private IGenericRepository<AnswerOption> _answerOptionRepository;
        private IGenericRepository<UserTest> _userTestRepository;
        private IGenericRepository<UserAnswer> _userAnswerRepository;
        private IGenericRepository<Appointment> _appointmentRepository;
        private IGenericRepository<PsychologistReview> _psychologistReviewRepository;
        private IGenericRepository<Payment> _paymentRepository;
        private IGenericRepository<UserSubscription> _userSubscriptionRepository;
        private IGenericRepository<SubscriptionPackage> _subscriptionPackageRepository;
        private IGenericRepository<Conversation> _conversationRepository;
        private IGenericRepository<Message> _messageRepository;
        private IGenericRepository<Participant> _participantRepository;
        private IGenericRepository<Promotion> _promotionRepository;
        private IGenericRepository<UserPromotion> _userPromotionRepository;
        private IGenericRepository<PatientProfile> _patientProfileRepository;
        private IGenericRepository<PsychologicalAssessment> _psychologicalAssessmentRepository;

        public UnitOfWork(TellMeDBContext dbContext, TellMeAuthDBContext dbAuthContext) 
        {
            _dbContext = dbContext;
            _dbAuthContext = dbAuthContext;
        }

        public IGenericRepository<Psychologist> PsychologistRepository => 
            _psychologistRepository ??= new GenericRepository<Psychologist>(_dbContext);
        public IGenericRepository<PsychologistEducation> PsychologistEducationRepository =>
           _psychologistEducationRepository ??= new GenericRepository<PsychologistEducation>(_dbContext);

        public IGenericRepository<PsychologistExperience> PsychologistExperienceRepository =>
            _psychologistExperienceRepository ??= new GenericRepository<PsychologistExperience>(_dbContext);

        public IGenericRepository<PsychologistLicenseCertification> PsychologistLicenseCertificationRepository =>
            _psychologistLicenseCertificationRepository ??= new GenericRepository<PsychologistLicenseCertification>(_dbContext);

        public IGenericRepository<PsychologicalTest> PsychologicalTestRepository =>
            _psychologicalTestRepository ??= new GenericRepository<PsychologicalTest>(_dbContext);

        public IGenericRepository<Question> QuestionRepository =>
            _questionRepository ??= new GenericRepository<Question>(_dbContext);

        public IGenericRepository<AnswerOption> AnswerOptionRepository =>
            _answerOptionRepository ??= new GenericRepository<AnswerOption>(_dbContext);

        public IGenericRepository<UserTest> UserTestRepository =>
            _userTestRepository ??= new GenericRepository<UserTest>(_dbContext);

        public IGenericRepository<UserAnswer> UserAnswerRepository =>
            _userAnswerRepository ??= new GenericRepository<UserAnswer>(_dbContext);

        public IGenericRepository<Appointment> AppointmentRepository => 
            _appointmentRepository ??= new GenericRepository<Appointment>(_dbContext);

        public IGenericRepository<PsychologistReview> PsychologistReviewRepository => 
            _psychologistReviewRepository ??= new GenericRepository<PsychologistReview>(_dbContext);

        public IGenericRepository<Payment> PaymentRepository => 
            _paymentRepository ??= new GenericRepository<Payment>(_dbContext);

        public IGenericRepository<UserSubscription> UserSubscriptionRepository => 
            _userSubscriptionRepository ??= new GenericRepository<UserSubscription>(_dbContext);

        public IGenericRepository<SubscriptionPackage> SubscriptionPackageRepository => 
            _subscriptionPackageRepository ??= new GenericRepository<SubscriptionPackage>(_dbContext);

        public IGenericRepository<Conversation> ConversationRepository =>
            _conversationRepository ??= new GenericRepository<Conversation>(_dbContext);

        public IGenericRepository<Message> MessageRepository =>
            _messageRepository ??= new GenericRepository<Message>(_dbContext);

        public IGenericRepository<Participant> ParticipantRepository =>
            _participantRepository ??= new GenericRepository<Participant>(_dbContext);

        public IGenericRepository<Promotion> PromotionRepository => 
            _promotionRepository ??= new GenericRepository<Promotion>(_dbContext);

        public IGenericRepository<UserPromotion> UserPromotionRepository =>
            _userPromotionRepository ??= new GenericRepository<UserPromotion>(_dbContext);

        public IGenericRepository<PatientProfile> PatientProfileRepository =>
            _patientProfileRepository ??= new GenericRepository<PatientProfile>(_dbContext);

        public IGenericRepository<PsychologicalAssessment> PsychologicalAssessmentRepository =>
            _psychologicalAssessmentRepository ??= new GenericRepository<PsychologicalAssessment>(_dbContext);

        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await Task.WhenAll(
                _dbContext.SaveChangesAsync()
            );
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
            _dbAuthContext?.Dispose();
        }
    }
}