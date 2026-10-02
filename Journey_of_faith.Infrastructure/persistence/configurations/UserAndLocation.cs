using Journey_of_faith.Domain.entities.location;
using Journey_of_faith.Domain.entities;
using Journey_of_faith.Domain.entities.notifications;
using Journey_of_faith.Infrastructure.identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Infrastructure.persistence.configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("User");

            builder.Property(u => u.Name).HasMaxLength(100).IsRequired();
            builder.Property(u => u.Avatar).HasMaxLength(255);
            builder.Property(u => u.CreationTime).HasDefaultValueSql("getdate()");
            builder.Property(u => u.LastModificationTime).HasDefaultValueSql("getdate()");
            builder.Property(u => u.IsDeleted).HasDefaultValue(false);

            // FK -> Church (SET NULL on delete)
            builder.HasOne(u => u.Church)
                .WithMany()
                .HasForeignKey(u => u.ChurchId)
                .OnDelete(DeleteBehavior.SetNull);

            // FK -> Province (CASCADE)
            builder.HasOne(u => u.Province)
                .WithMany()
                .HasForeignKey(u => u.ProvinceId)
                .OnDelete(DeleteBehavior.Cascade);

            // FK -> School (CASCADE)
            builder.HasOne(u => u.School)
                .WithMany()
                .HasForeignKey(u => u.SchoolId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

    public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
    {
        public void Configure(EntityTypeBuilder<Province> builder)
        {
            builder.ToTable("Province");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Property(p => p.Code).HasMaxLength(50);
            builder.Property(p => p.Type).HasMaxLength(50);
        }
    }

    public class DioceseConfiguration : IEntityTypeConfiguration<Diocese>
    {
        public void Configure(EntityTypeBuilder<Diocese> builder)
        {
            builder.ToTable("Diocese");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).HasMaxLength(10).IsRequired();
            builder.Property(d => d.Website).HasMaxLength(500);
            builder.Property(d => d.Address).HasMaxLength(300);
            builder.Property(d => d.Thumbnail).HasMaxLength(500);
            builder.Property(d => d.IsDeleted).HasDefaultValue(false);
            builder.Property(d => d.CreationTime).HasDefaultValueSql("getdate()");
            builder.Property(d => d.LastModificationTime).HasDefaultValueSql("getdate()");
            builder.Navigation(d => d.Churches)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }

    public class ChurchConfiguration : IEntityTypeConfiguration<Church>
    {
        public void Configure(EntityTypeBuilder<Church> builder)
        {
            builder.ToTable("Church");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).HasMaxLength(150).IsRequired();
            builder.Property(c => c.Thumbnail).HasMaxLength(300);
            builder.Property(c => c.Email).HasMaxLength(500);
            builder.Property(c => c.Address).HasMaxLength(255);
            builder.Property(c => c.IsDeleted).HasDefaultValue(false);
            builder.Property(c => c.CreationTime).HasDefaultValueSql("getdate()");
            builder.Property(c => c.LastModificationTime).HasDefaultValueSql("getdate()");

            builder.OwnsOne(c => c.GeoLocation, location =>
            {
                location.Property(value => value.Latitude)
                    .HasColumnName("Latitude");
                location.Property(value => value.Longitude)
                    .HasColumnName("Longitude");
            });
            builder.Navigation(c => c.GeoLocation).IsRequired();

            builder.HasOne(c => c.Diocese)
                .WithMany(d => d.Churches)
                .HasForeignKey(c => c.DioceseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(c => c.MassSchedules)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(c => c.LiveStreams)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(c => c.UserChurches)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(c => c.ChurchImages)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }

    public class SchoolLevelConfiguration : IEntityTypeConfiguration<SchoolLevel>
    {
        public void Configure(EntityTypeBuilder<SchoolLevel> builder)
        {
            builder.ToTable("SchoolLevel");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
        }
    }

    public class SchoolConfiguration : IEntityTypeConfiguration<School>
    {
        public void Configure(EntityTypeBuilder<School> builder)
        {
            builder.ToTable("School");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
            builder.Property(s => s.Thumbnail).HasMaxLength(500);
            builder.Property(s => s.Address).HasMaxLength(200);

            builder.HasOne(s => s.Level)
                .WithMany(sl => sl.Schools)
                .HasForeignKey(s => s.LevelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class UserChurchConfiguration : IEntityTypeConfiguration<UserChurch>
    {
        public void Configure(EntityTypeBuilder<UserChurch> builder)
        {
            builder.ToTable("UserChurch");
            builder.HasKey(uc => new { uc.UserId, uc.ChurchId });

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(uc => uc.Church)
                .WithMany(c => c.UserChurches)
                .HasForeignKey(uc => uc.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class UserActiveConfiguration : IEntityTypeConfiguration<UserActive>
    {
        public void Configure(EntityTypeBuilder<UserActive> build)
        {
            build.HasKey(e => e.Id);

            build.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(e => e.ApplicationUserId);
        }
    }
}
