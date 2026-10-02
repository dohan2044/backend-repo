
using Journey_of_faith.Domain.entities.catholic;
using Journey_of_faith.Domain.entities.location;
using Journey_of_faith.Domain.entities.masslive;
using Journey_of_faith.Domain.entities.notifications;
using Journey_of_faith.Domain.entities.prayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Journey_of_faith.Infrastructure.identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Infrastructure.persistence.configurations
{
    public class BibleBookConfiguration : IEntityTypeConfiguration<BibleBook>
    {
        public void Configure(EntityTypeBuilder<BibleBook> builder)
        {
            builder.ToTable("BibleBook");

            builder.HasKey(bb => bb.Id);

            builder.Property(bb => bb.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(bb => bb.Abbreviation)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(bb => bb.Testament)
                .HasMaxLength(2)
                .IsRequired();

            builder.Property(bb => bb.Category)
                .HasMaxLength(50);

            builder.Property(bb => bb.ChapterCount)
                .IsRequired();

            builder.HasIndex(bb => bb.Abbreviation)
                .IsUnique();
        }
    }
    public class BibleChapterConfiguration : IEntityTypeConfiguration<BibleChapter>
    {
        public void Configure(EntityTypeBuilder<BibleChapter> builder)
        {
            builder.ToTable("BibleChapter");

            builder.HasKey(bc => bc.Id);

            builder.Property(bc => bc.ChapterNumber)
                .IsRequired();

            builder.HasOne(bc => bc.Book)
                .WithMany(bb => bb.Chapters)
                .HasForeignKey(bc => bc.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(bc => new
            {
                bc.BookId,
                bc.ChapterNumber
            })
            .IsUnique();
        }
    }
    public class BibleVerseConfiguration : IEntityTypeConfiguration<BibleVerse>
    {
        public void Configure(EntityTypeBuilder<BibleVerse> builder)
        {
            builder.ToTable("BibleVerse");

            builder.HasKey(bv => bv.Id);

            builder.Property(bv => bv.VerseNumber)
                .IsRequired();

            builder.Property(bv => bv.TextVi)
                .IsRequired();

            builder.Property(bv => bv.TextEn);

            builder.HasOne(bv => bv.Chapter)
                .WithMany(bc => bc.Verses)
                .HasForeignKey(bv => bv.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(bv => new
            {
                bv.ChapterId,
                bv.VerseNumber
            })
            .IsUnique();
        }
    }
    public class MassTypeConfiguration : IEntityTypeConfiguration<MassType>
    {
        public void Configure(EntityTypeBuilder<MassType> builder)
        {
            builder.ToTable("MassType");
            builder.HasKey(mt => mt.Id);
            builder.Property(mt => mt.Name).HasMaxLength(150).IsRequired();
        }
    }

    public class MassScheduleConfiguration : IEntityTypeConfiguration<MassSchedule>
    {
        public void Configure(EntityTypeBuilder<MassSchedule> builder)
        {
            builder.ToTable("MassSchedule");
            builder.HasKey(ms => ms.Id);
            builder.Property(ms => ms.IsDeleted).HasDefaultValue(false);
            builder.Property(ms => ms.CreationTime).HasDefaultValueSql("getdate()");
            builder.Property(ms => ms.LastModificationTime).HasDefaultValueSql("getdate()");

            builder.HasOne(ms => ms.Church)
                .WithMany(c => c.MassSchedules)
                .HasForeignKey(ms => ms.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ms => ms.MassType)
                .WithMany(mt => mt.MassSchedules)
                .HasForeignKey(ms => ms.MassTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class MassVideoConfiguration : IEntityTypeConfiguration<MassVideo>
    {
        public void Configure(EntityTypeBuilder<MassVideo> builder)
        {
            builder.ToTable("MassVideo");
            builder.HasKey(mv => mv.Id);
            builder.Property(mv => mv.Url).IsRequired();
            builder.Property(mv => mv.Title).HasMaxLength(300).IsRequired();
            builder.Property(mv => mv.Description).HasMaxLength(500);
            builder.Property(mv => mv.IsDeleted).HasDefaultValue(false);
            builder.Property(mv => mv.CreationTime).HasDefaultValueSql("getdate()");
            builder.Property(mv => mv.LastModificationTime).HasDefaultValueSql("getdate()");
        }
    }

    public class LiveStreamConfiguration : IEntityTypeConfiguration<LiveStream>
    {
        public void Configure(EntityTypeBuilder<LiveStream> builder)
        {
            builder.ToTable("LiveStream");
            builder.HasKey(ls => ls.Id);
            builder.Property(ls => ls.Title).HasMaxLength(200).IsRequired();
            builder.Property(ls => ls.YouTubeUrl).HasMaxLength(500).IsRequired();
            builder.Property(ls => ls.Description).HasMaxLength(1000);
            builder.Property(ls => ls.ThumbnailUrl).HasMaxLength(500);
            builder.Property(ls => ls.CreatedAt).HasDefaultValueSql("getdate()");

            builder.HasOne(ls => ls.Church)
                .WithMany(c => c.LiveStreams)
                .HasForeignKey(ls => ls.ChurchId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class CatholicFeastConfiguration : IEntityTypeConfiguration<CatholicFeast>
    {
        public void Configure(EntityTypeBuilder<CatholicFeast> builder)
        {
            builder.ToTable("CatholicFeast");
            builder.HasKey(cf => cf.Id);
            builder.Property(cf => cf.Name).HasMaxLength(150).IsRequired();
            builder.Property(cf => cf.Description).HasMaxLength(500);
            builder.Property(cf => cf.IsDeleted).HasDefaultValue(false);
            builder.Property(cf => cf.CreationTime).HasDefaultValueSql("getdate()");
            builder.Property(cf => cf.LastModificationTime).HasDefaultValueSql("getdate()");
        }
    }

    public class DailyWordConfiguration : IEntityTypeConfiguration<DailyWord>
    {
        public void Configure(EntityTypeBuilder<DailyWord> builder)
        {
            builder.ToTable("DailyWord");
            builder.HasKey(dw => dw.Id);
            builder.Property(dw => dw.Title).HasMaxLength(200);
            builder.Property(dw => dw.Gospel).HasMaxLength(200);
            builder.Property(dw => dw.BibleContent).IsRequired();
            builder.Property(dw => dw.IsDeleted).HasDefaultValue(false);
            builder.Property(dw => dw.CreationTime).HasDefaultValueSql("getdate()");
            builder.Property(dw => dw.LastModificationTime).HasDefaultValueSql("getdate()");
        }
    }

    public class PrayerRequestConfiguration : IEntityTypeConfiguration<PrayerRequest>
    {
        public void Configure(EntityTypeBuilder<PrayerRequest> builder)
        {
            builder.ToTable("PrayerRequest");
            builder.HasKey(pr => pr.Id);
            builder.Property(pr => pr.Title).HasMaxLength(300);
            builder.Property(pr => pr.RequestContent).HasMaxLength(500);
            builder.Property(pr => pr.IsDeleted).HasDefaultValue(false);
            builder.Property(pr => pr.CreationTime).HasDefaultValueSql("getdate()");
            builder.Property(pr => pr.LastModificationTime).HasDefaultValueSql("getdate()");

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class PrayerCommentConfiguration : IEntityTypeConfiguration<PrayerComment>
    {
        public void Configure(EntityTypeBuilder<PrayerComment> builder)
        {
            builder.ToTable("PrayerComment");
            builder.HasKey(pc => pc.Id);
            builder.Property(pc => pc.CommentContent).HasMaxLength(500).IsRequired();
            builder.Property(pc => pc.IsDeleted).HasDefaultValue(false);
            builder.Property(pc => pc.CreationTime).HasDefaultValueSql("getdate()");
            builder.Property(pc => pc.LastModificationTime).HasDefaultValueSql("getdate()");

            builder.HasOne(pc => pc.PrayerRequest)
                .WithMany(pr => pr.Comments)
                .HasForeignKey(pc => pc.PrayerRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(pc => pc.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
    {
        public void Configure(EntityTypeBuilder<DeviceToken> builder)
        {
            builder.ToTable("DeviceToken");
            builder.HasKey(dt => dt.Id);
            builder.Property(dt => dt.Token).HasMaxLength(500).IsRequired();
            builder.Property(dt => dt.Platform).HasMaxLength(20);
            builder.Property(dt => dt.CreatedAt).HasDefaultValueSql("getdate()");

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(dt => dt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
    {
        public void Configure(EntityTypeBuilder<NotificationPreference> builder)
        {
            builder.ToTable("NotificationPreference");
            builder.HasKey(np => np.Id);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(np => np.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ReminderSettingConfiguration : IEntityTypeConfiguration<ReminderSetting>
    {
        public void Configure(EntityTypeBuilder<ReminderSetting> builder)
        {
            builder.ToTable("ReminderSetting");
            builder.HasKey(rs => rs.Id);
            builder.Property(rs => rs.SpeechGender).HasMaxLength(50);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(rs => rs.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ChurchImageConfiguration : IEntityTypeConfiguration<ChurchImage>
    {
        public void Configure(EntityTypeBuilder<ChurchImage> builder)
        {
            builder.ToTable("ChurchImages");
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.Church)
                .WithMany(e => e.ChurchImages)
                .HasForeignKey(e => e.ChurchId)
                .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        }
    }

}
