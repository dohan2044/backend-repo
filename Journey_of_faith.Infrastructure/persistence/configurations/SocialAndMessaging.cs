
using Journey_of_faith.Domain.entities.social;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Journey_of_faith.Infrastructure.identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Infrastructure.persistence.configurations
{
    public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> builder)
        {
            builder.ToTable("Friendship");
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Status).HasMaxLength(20).IsRequired();
            builder.Property(f => f.IsDeleted).HasDefaultValue(false);
            builder.Property(f => f.CreationTime).HasDefaultValueSql("getdate()");
            builder.Property(f => f.LastModificationTime).HasDefaultValueSql("getdate()");

            // NoAction để tránh multiple cascade paths
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("Group");
            builder.HasKey(g => g.Id);
            builder.Property(g => g.Name).HasMaxLength(200);
            builder.Property(g => g.Description).HasMaxLength(300);
            builder.Property(g => g.Avatar).HasMaxLength(500);
            builder.Property(g => g.GroupType).HasMaxLength(50);
            builder.Property(g => g.Privacy).HasMaxLength(50);
            builder.Property(g => g.IsDeleted).HasDefaultValue(false);
            builder.Property(g => g.CreationTime).HasDefaultValueSql("getdate()");
            builder.Property(g => g.LastModificationTime).HasDefaultValueSql("getdate()");
        }
    }

    public class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
    {
        public void Configure(EntityTypeBuilder<GroupMember> builder)
        {
            builder.ToTable("GroupMember");
            builder.HasKey(gm => gm.Id);

            builder.HasOne(gm => gm.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.ToTable("Conversation");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Avatar).HasMaxLength(500);

            builder.HasOne(c => c.Group)
                .WithMany(g => g.Conversations)
                .HasForeignKey(c => c.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(c => c.CreatorUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
    {
        public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
        {
            builder.ToTable("ConversationParticipant");
            builder.HasKey(cp => cp.Id);

            builder.HasOne(cp => cp.Conversation)
                .WithMany(c => c.Participants)
                .HasForeignKey(cp => cp.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(cp => cp.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Message");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.MessageContent).IsRequired();
            builder.Property(m => m.MessageType).HasDefaultValue(0);
            builder.Property(m => m.IsDeleted).HasDefaultValue(false);
            builder.Property(m => m.CreationTime).HasDefaultValueSql("getdate()");

            builder.HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(m => m.FromUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class MessageAttachmentConfiguration : IEntityTypeConfiguration<MessageAttachment>
    {
        public void Configure(EntityTypeBuilder<MessageAttachment> builder)
        {
            builder.ToTable("MessageAttachment");
            builder.HasKey(ma => ma.Id);
            builder.Property(ma => ma.FileUrl).HasMaxLength(500);
            builder.Property(ma => ma.FileName).HasMaxLength(255);
            builder.Property(ma => ma.FileType).HasMaxLength(50);

            builder.HasOne(ma => ma.Message)
                .WithMany(m => m.Attachments)
                .HasForeignKey(ma => ma.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class MessageReactionConfiguration : IEntityTypeConfiguration<MessageReaction>
    {
        public void Configure(EntityTypeBuilder<MessageReaction> builder)
        {
            builder.ToTable("MessageReaction");
            builder.HasKey(mr => mr.Id);
            builder.Property(mr => mr.Reaction).HasMaxLength(50);

            builder.HasOne(mr => mr.Message)
                .WithMany(m => m.Reactions)
                .HasForeignKey(mr => mr.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(mr => mr.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class MessageStatusConfiguration : IEntityTypeConfiguration<MessageStatus>
    {
        public void Configure(EntityTypeBuilder<MessageStatus> builder)
        {
            builder.ToTable("MessageStatus");
            builder.HasKey(ms => ms.Id);
            builder.Property(ms => ms.UpdateTime).HasDefaultValueSql("getutcdate()");

            builder.HasOne(ms => ms.Message)
                .WithMany(m => m.Statuses)
                .HasForeignKey(ms => ms.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class GroupEventConfiguration : IEntityTypeConfiguration<GroupEvent>
    {
        public void Configure(EntityTypeBuilder<GroupEvent> builder)
        {
            builder.ToTable("GroupEvent");
            builder.HasKey(ge => ge.Id);

            builder.HasOne(ge => ge.Conversation)
                .WithMany(c => c.GroupEvents)
                .HasForeignKey(ge => ge.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
