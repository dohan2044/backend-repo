using Journey_of_faith.Infrastructure.identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Infrastructure.persistence.configurations
{
    internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken> 
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder )
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Token).HasMaxLength(200);
            builder.HasIndex(r => r.Token).IsUnique();

            builder.HasOne(r => r.User).WithMany().HasForeignKey(e => e.UserId);
        }
    }
}
