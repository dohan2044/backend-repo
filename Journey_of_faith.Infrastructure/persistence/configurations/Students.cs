using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class FaithPrayerCategoryConfiguration : IEntityTypeConfiguration<FaithPrayerCategory>
{
    public void Configure(EntityTypeBuilder<FaithPrayerCategory> builder)
    {
        builder.ToTable("FaithPrayerCategories");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.SortOrder).HasDefaultValue(0);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.CreatorUserId).HasDefaultValue(Guid.Empty);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasMany(x => x.Prayers)
               .WithOne(p => p.Category)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class FaithPrayerConfiguration : IEntityTypeConfiguration<FaithPrayer>
{
    public void Configure(EntityTypeBuilder<FaithPrayer> builder)
    {
        builder.ToTable("FaithPrayers");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(300);
        builder.Property(x => x.Subtitle).HasMaxLength(500);
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.Source).HasMaxLength(300);
        builder.Property(x => x.SortOrder).HasDefaultValue(0);
        builder.Property(x => x.IsPublished).HasDefaultValue(false);
        builder.Property(x => x.CreatorUserId).HasDefaultValue(Guid.Empty);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(x => new { x.CategoryId, x.IsPublished, x.IsDeleted })
               .HasDatabaseName("IX_FaithPrayers_CategoryId");
    }
}

public class StudentGroupConfiguration :  IEntityTypeConfiguration<StudentGroup>
{
    public void Configure(EntityTypeBuilder<StudentGroup> builder)
    {
        builder.ToTable("StudentGroups");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.LogoUrl).HasMaxLength(500);
        builder.Property(x => x.ContactEmail).HasMaxLength(200);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.CreatorUserId).HasDefaultValue(Guid.Empty);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UQ_StudentGroups_Code");

        builder.HasMany(x => x.Members)
               .WithOne(m => m.Group)
               .HasForeignKey(m => m.GroupId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class StudentGroupMemberConfiguration : IEntityTypeConfiguration<StudentGroupMember>
{
    public void Configure(EntityTypeBuilder<StudentGroupMember> builder)
    {
        builder.ToTable("StudentGroupMembers");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Role).IsRequired().HasMaxLength(50).HasDefaultValue("Member");
        builder.Property(x => x.JoinedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(x => x.Status).HasDefaultValue((byte)0);
        builder.Property(x => x.CreatorUserId).HasDefaultValue(Guid.Empty);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(x => new { x.GroupId, x.UserId })
               .IsUnique()
               .HasDatabaseName("UQ_StudentGroupMembers_Group_User");

        builder.HasIndex(x => new { x.UserId, x.Status })
               .HasDatabaseName("IX_StudentGroupMembers_UserId");
    }
}

public class PrayerIntentionConfiguration : IEntityTypeConfiguration<PrayerIntention>
{
    public void Configure(EntityTypeBuilder<PrayerIntention> builder)
    {
        builder.ToTable("PrayerIntentions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.IsAnonymous).HasDefaultValue(false);
        builder.Property(x => x.Status).HasDefaultValue((byte)0);
        builder.Property(x => x.PrayerCount).HasDefaultValue(0);
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(x => x.CreatorUserId).HasDefaultValue(Guid.Empty);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(x => new { x.Status, x.CreatedAt })
               .HasDatabaseName("IX_PrayerIntentions_Status_CreatedAt");

        builder.HasMany(x => x.PrayingUsers)
               .WithOne(p => p.Intention)
               .HasForeignKey(p => p.IntentionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PrayerIntentionPrayerConfiguration : IEntityTypeConfiguration<PrayerIntentionPrayer>
{
    public void Configure(EntityTypeBuilder<PrayerIntentionPrayer> builder)
    {
        builder.ToTable("PrayerIntentionPrayers");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(x => x.CreatorUserId).HasDefaultValue(Guid.Empty);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(x => new { x.IntentionId, x.UserId })
               .IsUnique()
               .HasDatabaseName("UQ_PrayerIntentionPrayers_Intention_User");

        builder.HasIndex(x => x.UserId)
               .HasDatabaseName("IX_PrayerIntentionPrayers_UserId");
    }
}

public class CharityActivityConfiguration : IEntityTypeConfiguration<CharityActivity>
{
    public void Configure(EntityTypeBuilder<CharityActivity> builder)
    {
        builder.ToTable("CharityActivities");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(300);
        builder.Property(x => x.Description).IsRequired();
        builder.Property(x => x.Location).IsRequired().HasMaxLength(300);
        builder.Property(x => x.StartAt).IsRequired();
        builder.Property(x => x.EndAt).IsRequired();
        builder.Property(x => x.MaxParticipants).HasDefaultValue(null);
        builder.Property(x => x.Status).HasDefaultValue((byte)0);
        builder.Property(x => x.CoverImageUrl).HasMaxLength(500);
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(x => x.CreatorUserId).HasDefaultValue(Guid.Empty);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(x => new { x.Status, x.StartAt })
               .HasDatabaseName("IX_CharityActivities_Status_StartAt");

        builder.HasMany(x => x.Registrations)
               .WithOne(r => r.Activity)
               .HasForeignKey(r => r.ActivityId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
public class CharityRegistrationConfiguration : IEntityTypeConfiguration<CharityRegistration>
{
    public void Configure(EntityTypeBuilder<CharityRegistration> builder)
    {
        builder.ToTable("CharityRegistrations");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RegisteredAt).HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(x => x.Status).HasDefaultValue((byte)0);
        builder.Property(x => x.Note).HasMaxLength(500);
        builder.Property(x => x.CreatorUserId).HasDefaultValue(Guid.Empty);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(x => new { x.ActivityId, x.UserId })
               .IsUnique()
               .HasDatabaseName("UQ_CharityRegistrations_Activity_User");

        builder.HasIndex(x => new { x.UserId, x.Status })
               .HasDatabaseName("IX_CharityRegistrations_UserId");
    }
}

public class StudentEventConfiguration : IEntityTypeConfiguration<StudentEvent>
{
    public void Configure(EntityTypeBuilder<StudentEvent> builder)
    {
        builder.ToTable("StudentEvents");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(300);
        builder.Property(x => x.Description).IsRequired();
        builder.Property(x => x.Location).IsRequired().HasMaxLength(300);
        builder.Property(x => x.Status).HasDefaultValue((byte)0);
        builder.Property(x => x.CoverImageUrl).HasMaxLength(500);
        builder.Property(x => x.CreatorUserId).HasDefaultValue(Guid.Empty);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(x => new { x.Status, x.StartAt })
               .HasDatabaseName("IX_StudentEvents_Status_StartAt");

        builder.HasMany(x => x.Registrations)
               .WithOne(r => r.Event)
               .HasForeignKey(r => r.EventId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class StudentEventRegistrationConfiguration : IEntityTypeConfiguration<StudentEventRegistration>
{
    public void Configure(EntityTypeBuilder<StudentEventRegistration> builder)
    {
        builder.ToTable("StudentEventRegistrations");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RegisteredAt).HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(x => x.Status).HasDefaultValue((byte)0);
        builder.Property(x => x.CreatorUserId).HasDefaultValue(Guid.Empty);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(x => new { x.EventId, x.UserId })
               .IsUnique()
               .HasDatabaseName("UQ_StudentEventRegistrations_Event_User");

        builder.HasIndex(x => new { x.UserId, x.Status })
               .HasDatabaseName("IX_StudentEventRegistrations_UserId");
    }
}