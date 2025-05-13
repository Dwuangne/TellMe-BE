using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Enities;
using TellMe.Repository.Repositories;
using TellMe.Repository.SMTPs.Repositories;

namespace TellMe.Repository.Infrastructures
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Psychologist> PsychologistRepository { get; }
        IGenericRepository<PsychologistEducation> PsychologistEducationRepository { get; }
        IGenericRepository<PsychologistExperience> PsychologistExperienceRepository { get; }
        IGenericRepository<PsychologistLicenseCertification> PsychologistLicenseCertificationRepository { get; }
        IGenericRepository<PsychologicalTest> PsychologicalTestRepository { get; }
        IGenericRepository<Question> QuestionRepository { get; }
        IGenericRepository<AnswerOption> AnswerOptionRepository { get; }
        IGenericRepository<UserTest> UserTestRepository { get; }
        IGenericRepository<UserAnswer> UserAnswerRepository { get; }

        void Commit();
        Task CommitAsync();
    }
}
