using Microsoft.Extensions.Configuration;
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