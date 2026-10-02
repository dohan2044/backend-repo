using Journey_of_faith.Domain.entities.quiz;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Journey_of_faith.Infrastructure.identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Infrastructure.persistence.configurations
{
    public class QuizLevelConfiguration : IEntityTypeConfiguration<QuizLevel>
    {
        public void Configure(EntityTypeBuilder<QuizLevel> builder)
        {
            builder.ToTable("QuizLevel");
            builder.HasKey(ql => ql.Id);
            builder.Property(ql => ql.Name).HasMaxLength(50).IsRequired();
        }
    }

    public class QuestionTypeConfiguration : IEntityTypeConfiguration<QuestionType>
    {
        public void Configure(EntityTypeBuilder<QuestionType> builder)
        {
            builder.ToTable("QuestionType");
            builder.HasKey(qt => qt.Id);
            builder.Property(qt => qt.Name).HasMaxLength(50).IsRequired();
        }
    }

    public class QuestionCategoryConfiguration : IEntityTypeConfiguration<QuestionCategory>
    {
        public void Configure(EntityTypeBuilder<QuestionCategory> builder)
        {
            builder.ToTable("QuestionCategory");
            builder.HasKey(qc => qc.Id);
            builder.Property(qc => qc.Name).HasMaxLength(50).IsRequired();
        }
    }

    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Question");
            builder.HasKey(q => q.Id);
            builder.Property(q => q.QuestionContent).IsRequired();
            builder.Property(q => q.ImageUrl).HasMaxLength(500);
            builder.Property(q => q.CreatedTime).HasDefaultValueSql("getutcdate()");

            builder.HasOne(q => q.Level)
                .WithMany(ql => ql.Questions)
                .HasForeignKey(q => q.LevelId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(q => q.Type)
                .WithMany(qt => qt.Questions)
                .HasForeignKey(q => q.TypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(q => q.Category)
                .WithMany(qc => qc.Questions)
                .HasForeignKey(q => q.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.ToTable("Answer");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Content).HasMaxLength(500).IsRequired();
            builder.Property(a => a.ImageUrl).HasMaxLength(500);

            builder.HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.ToTable("Quiz");
            builder.HasKey(q => q.Id);
            builder.Property(q => q.Title).HasMaxLength(300);
            builder.Property(q => q.Description).HasMaxLength(500);
        }
    }

    public class QuizQuestionConfiguration : IEntityTypeConfiguration<QuizQuestion>
    {
        public void Configure(EntityTypeBuilder<QuizQuestion> builder)
        {
            builder.ToTable("QuizQuestion");
            builder.HasKey(qq => qq.Id);

            builder.HasOne(qq => qq.Quiz)
                .WithMany(q => q.QuizQuestions)
                .HasForeignKey(qq => qq.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(qq => qq.Question)
                .WithMany(q => q.QuizQuestions)
                .HasForeignKey(qq => qq.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
    {
        public void Configure(EntityTypeBuilder<QuizAttempt> builder)
        {
            builder.ToTable("QuizAttempt");
            builder.HasKey(qa => qa.Id);

            builder.HasOne(qa => qa.Quiz)
                .WithMany(q => q.Attempts)
                .HasForeignKey(qa => qa.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(qa => qa.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class AttemptAnswerConfiguration : IEntityTypeConfiguration<AttemptAnswer>
    {
        public void Configure(EntityTypeBuilder<AttemptAnswer> builder)
        {
            builder.ToTable("AttemptAnswer");
            builder.HasKey(aa => aa.Id);

            builder.HasOne(aa => aa.Attempt)
                .WithMany(qa => qa.AttemptAnswers)
                .HasForeignKey(aa => aa.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
