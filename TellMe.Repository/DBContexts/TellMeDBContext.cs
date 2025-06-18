using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TellMe.Repository.Enities;

namespace TellMe.Repository.DBContexts
{
    public class TellMeDBContext : DbContext
    {
        public DbSet<Psychologist> Psychologists { get; set; }
        public DbSet<PsychologistEducation> PsychologistEducations { get; set; }
        public DbSet<PsychologistExperience> PsychologistExperiences { get; set; }
        public DbSet<PsychologistLicenseCertification> PsychologistLicenseCertifications { get; set; }
        public DbSet<PsychologicalTest> PsychologicalTests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<UserTest> UserTests { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<PsychologistReview> PsychologistReviews { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<SubscriptionPackage> SubscriptionPackages { get; set; }

        public DbSet<Conversation> Conversation { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Participant> Participant { get; set; }

        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<UserPromotion> UserPromotions { get; set; }

        public TellMeDBContext()
        {
        }

        public TellMeDBContext(DbContextOptions<TellMeDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.UserTest)
                .WithMany(ut => ut.UserAnswers)
                .HasForeignKey(ua => ua.UserTestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Question)
                .WithMany(q => q.UserAnswers)
                .HasForeignKey(ua => ua.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.AnswerOption)
                .WithMany(ao => ao.UserAnswers)
                .HasForeignKey(ua => ua.AnswerOptionId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Payment-Appointment relationship
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Appointment)
                .WithOne(a => a.Payment)
                .HasForeignKey<Appointment>(a => a.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Payment-UserSubscription relationship
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.UserSubscription)
                .WithOne(s => s.Payment)
                .HasForeignKey<UserSubscription>(s => s.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
