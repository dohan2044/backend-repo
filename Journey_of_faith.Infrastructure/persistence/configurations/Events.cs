using Journey_of_faith.Domain.entities.events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Journey_of_faith.Infrastructure.identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Infrastructure.persistence.configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Event");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Title).HasMaxLength(200).IsRequired();
            builder.Property(e => e.Description).HasMaxLength(2000);
            builder.Property(e => e.Location).HasMaxLength(255);
            builder.Property(e => e.ImageUrl).HasMaxLength(500);
            builder.Property(e => e.IsDeleted).HasDefaultValue(false);
            builder.Property(e => e.CreationTime).HasDefaultValueSql("getdate()");
            builder.Property(e => e.LastModificationTime).HasDefaultValueSql("getdate()");
        }
    }

    public class EventCategoryConfiguration : IEntityTypeConfiguration<EventCategory>
    {
        public void Configure(EntityTypeBuilder<EventCategory> builder)
        {
            builder.ToTable("EventCategory");
            builder.HasKey(ec => ec.Id);
            builder.Property(ec => ec.Name).HasMaxLength(200).IsRequired();
        }
    }

    public class EventCategoryMappingConfiguration : IEntityTypeConfiguration<EventCategoryMapping>
    {
        public void Configure(EntityTypeBuilder<EventCategoryMapping> builder)
        {
            builder.ToTable("EventCategoryMapping");
            builder.HasKey(ecm => ecm.Id);

            builder.HasOne(ecm => ecm.Event)
                .WithMany(e => e.CategoryMappings)
                .HasForeignKey(ecm => ecm.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ecm => ecm.Category)
                .WithMany(ec => ec.EventMappings)
                .HasForeignKey(ecm => ecm.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class EventCommentConfiguration : IEntityTypeConfiguration<EventComment>
    {
        public void Configure(EntityTypeBuilder<EventComment> builder)
        {
            builder.ToTable("EventComment");
            builder.HasKey(ec => ec.Id);
            builder.Property(ec => ec.Comment).IsRequired();

            builder.HasOne(ec => ec.Event)
                .WithMany(e => e.Comments)
                .HasForeignKey(ec => ec.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(ec => ec.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class EventFollowerConfiguration : IEntityTypeConfiguration<EventFollower>
    {
        public void Configure(EntityTypeBuilder<EventFollower> builder)
        {
            builder.ToTable("EventFollower");
            builder.HasKey(ef => ef.Id);
            builder.Property(ef => ef.FollowedTime).HasDefaultValueSql("getutcdate()");

            builder.HasOne(ef => ef.Event)
                .WithMany(e => e.Followers)
                .HasForeignKey(ef => ef.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(ef => ef.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class EventParticipantConfiguration : IEntityTypeConfiguration<EventParticipant>
    {
        public void Configure(EntityTypeBuilder<EventParticipant> builder)
        {
            builder.ToTable("EventParticipant");
            builder.HasKey(ep => ep.Id);
            builder.Property(ep => ep.RegisteredTime).HasDefaultValueSql("getutcdate()");

            builder.HasOne(ep => ep.Event)
                .WithMany(e => e.Participants)
                .HasForeignKey(ep => ep.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(ep => ep.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class EventImageConfiguration : IEntityTypeConfiguration<EventImage>
    {
        public void Configure(EntityTypeBuilder<EventImage> builder)
        {
            builder.ToTable("EventImage");
            builder.HasKey(ei => ei.Id);
            builder.Property(ei => ei.ImageUrl).IsRequired();

            builder.HasOne(ei => ei.Event)
                .WithMany(e => e.Images)
                .HasForeignKey(ei => ei.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class EventNotificationConfiguration : IEntityTypeConfiguration<EventNotification>
    {
        public void Configure(EntityTypeBuilder<EventNotification> builder)
        {
            builder.ToTable("EventNotification");
            builder.HasKey(en => en.Id);
            builder.Property(en => en.Title).HasMaxLength(255).IsRequired();
            builder.Property(en => en.NotifyContent).HasMaxLength(500).IsRequired();
            builder.Property(en => en.CreatedTime).HasDefaultValueSql("getutcdate()");

            builder.HasOne(en => en.Event)
                .WithMany(e => e.Notifications)
                .HasForeignKey(en => en.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class UserEventConfiguration : IEntityTypeConfiguration<UserEvent>
    {
        public void Configure(EntityTypeBuilder<UserEvent> builder)
        {
            builder.ToTable("UserEvent");
            builder.HasKey(ue => new { ue.UserId, ue.EventId });
            builder.Property(ue => ue.FollowedAt).HasDefaultValueSql("getdate()");

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(ue => ue.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ue => ue.Event)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(ue => ue.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
