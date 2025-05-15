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
        IGenericRepository<Appointment> AppointmentRepository { get; }
        IGenericRepository<PsychologistReview> PsychologistReviewRepository { get; }
        IGenericRepository<Payment> PaymentRepository { get; }
        IGenericRepository<UserSubscription> UserSubscriptionRepository { get; }
        IGenericRepository<SubscriptionPackage> SubscriptionPackageRepository { get; }

        void Commit();
        Task CommitAsync();
    }
}
