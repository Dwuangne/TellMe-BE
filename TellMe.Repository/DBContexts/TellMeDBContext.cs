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
                .OnDelete(DeleteBehavior.Restrict); // Hoặc .NoAction để tránh CASCADE

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Question)
                .WithMany(q => q.UserAnswers)
                .HasForeignKey(ua => ua.QuestionId)
                .OnDelete(DeleteBehavior.NoAction); // Nếu bạn cần cascade ở đây

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.AnswerOption)
                .WithMany(ao => ao.UserAnswers)
                .HasForeignKey(ua => ua.AnswerOptionId)
                .OnDelete(DeleteBehavior.SetNull); // hoặc Restrict tùy ý bạn
        }

    }
}
