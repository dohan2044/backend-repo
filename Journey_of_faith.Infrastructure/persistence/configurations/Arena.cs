using Journey_of_faith.Domain.entities.compete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Journey_of_faith.Infrastructure.persistence.configurations;

public class ArenaLevelConfiguration : IEntityTypeConfiguration<ArenaLevels>
{
    public void Configure(EntityTypeBuilder<ArenaLevels> builder)
    {
        builder.ToTable("ArenaLevel");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.LevelCode).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.LevelCode).IsUnique();

        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Subtitle).HasMaxLength(300);
        builder.Property(x => x.Tag).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.FocusTopics).HasMaxLength(500);
        builder.Property(x => x.ComboMultiplier).HasColumnType("decimal(5,2)");

        builder.HasOne(x => x.PrerequisiteLevel)
            .WithMany()
            .HasForeignKey(x => x.PrerequisiteLevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ArenaQuestionConfiguration : IEntityTypeConfiguration<ArenaQuestion>
{
    public void Configure(EntityTypeBuilder<ArenaQuestion> builder)
    {
        builder.ToTable("ArenaQuestion");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.Explanation).HasMaxLength(1000);

        builder.HasOne(x => x.Level)
            .WithMany(l => l.Questions)
            .HasForeignKey(x => x.LevelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ArenaQuestionOptionConfiguration : IEntityTypeConfiguration<ArenaQuestionOption>
{
    public void Configure(EntityTypeBuilder<ArenaQuestionOption> builder)
    {
        builder.ToTable("ArenaQuestionOption");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content).HasMaxLength(500).IsRequired();

        builder.HasOne(x => x.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ArenaAttemptConfiguration : IEntityTypeConfiguration<ArenaAttempt>
{
    public void Configure(EntityTypeBuilder<ArenaAttempt> builder)
    {
        builder.ToTable("ArenaAttempt");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Level)
            .WithMany()
            .HasForeignKey(x => x.LevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ArenaAttemptAnswerConfiguration : IEntityTypeConfiguration<ArenaAttemptAnswer>
{
    public void Configure(EntityTypeBuilder<ArenaAttemptAnswer> builder)
    {
        builder.ToTable("ArenaAttemptAnswer");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Attempt)
            .WithMany(a => a.Answers)
            .HasForeignKey(x => x.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Question)
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SelectedOption)
            .WithMany()
            .HasForeignKey(x => x.SelectedOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ArenaUserProgressConfiguration : IEntityTypeConfiguration<ArenaUserProgress>
{
    public void Configure(EntityTypeBuilder<ArenaUserProgress> builder)
    {
        builder.ToTable("ArenaUserProgress");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.UserId, x.LevelId }).IsUnique();

        builder.HasOne(x => x.Level)
            .WithMany()
            .HasForeignKey(x => x.LevelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ArenaAchievementConfiguration : IEntityTypeConfiguration<ArenaAchievement>
{
    public void Configure(EntityTypeBuilder<ArenaAchievement> builder)
    {
        builder.ToTable("ArenaAchievement");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.Property(x => x.IconKey).HasMaxLength(100);
    }
}

public class ArenaUserAchievementConfiguration : IEntityTypeConfiguration<ArenaUserAchievement>
{
    public void Configure(EntityTypeBuilder<ArenaUserAchievement> builder)
    {
        builder.ToTable("ArenaUserAchievement");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.UserId, x.AchievementId }).IsUnique();

        builder.HasOne(x => x.Achievement)
            .WithMany()
            .HasForeignKey(x => x.AchievementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
