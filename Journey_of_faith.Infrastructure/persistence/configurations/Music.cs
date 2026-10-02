
using Journey_of_faith.Domain.entities.musics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Journey_of_faith.Infrastructure.identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Infrastructure.persistence.configurations
{
    public class ArtistConfiguration : IEntityTypeConfiguration<Artist>
    {
        public void Configure(EntityTypeBuilder<Artist> builder)
        {
            builder.ToTable("Artist");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Name).HasMaxLength(200).IsRequired();
            builder.Property(a => a.Description).HasMaxLength(200).IsRequired();
            builder.Property(a => a.ImageUrl).HasMaxLength(500);
        }
    }

    public class AlbumConfiguration : IEntityTypeConfiguration<Album>
    {
        public void Configure(EntityTypeBuilder<Album> builder)
        {
            builder.ToTable("Album");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Title).HasMaxLength(255).IsRequired();
            builder.Property(a => a.CoverImageUrl).HasMaxLength(500);

            builder.HasOne(a => a.Artist)
                .WithMany(ar => ar.Albums)
                .HasForeignKey(a => a.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class SongConfiguration : IEntityTypeConfiguration<Song>
    {
        public void Configure(EntityTypeBuilder<Song> builder)
        {
            builder.ToTable("Song");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Title).HasMaxLength(255).IsRequired();
            builder.Property(s => s.AudioUrl).HasMaxLength(500);
            builder.Property(s => s.CoverImageUrl).HasMaxLength(500);
            builder.Property(s => s.IsDeleted).HasDefaultValue(false);
            builder.Property(s => s.CreationTime).HasDefaultValueSql("getdate()");
            builder.Property(s => s.LastModificationTime).HasDefaultValueSql("getdate()");

            builder.HasOne(s => s.Artist)
                .WithMany(a => a.Songs)
                .HasForeignKey(s => s.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            // Album FK không có cascade (để tránh xung đột với Artist cascade)
            builder.HasOne(s => s.Album)
                .WithMany(a => a.Songs)
                .HasForeignKey(s => s.AlbumId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class SongCategoryConfiguration : IEntityTypeConfiguration<SongCategory>
    {
        public void Configure(EntityTypeBuilder<SongCategory> builder)
        {
            builder.ToTable("SongCategory");
            builder.HasKey(sc => sc.Id);
            builder.Property(sc => sc.Name).HasMaxLength(200);
        }
    }

    public class SongCategoryMappingConfiguration : IEntityTypeConfiguration<SongCategoryMapping>
    {
        public void Configure(EntityTypeBuilder<SongCategoryMapping> builder)
        {
            builder.ToTable("SongCategoryMapping");
            builder.HasKey(scm => scm.Id);

            builder.HasOne(scm => scm.Song)
                .WithMany(s => s.CategoryMappings)
                .HasForeignKey(scm => scm.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(scm => scm.Category)
                .WithMany(sc => sc.SongMappings)
                .HasForeignKey(scm => scm.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
    {
        public void Configure(EntityTypeBuilder<Playlist> builder)
        {
            builder.ToTable("Playlist");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Title).HasMaxLength(200).IsRequired();

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class PlaylistSongConfiguration : IEntityTypeConfiguration<PlaylistSong>
    {
        public void Configure(EntityTypeBuilder<PlaylistSong> builder)
        {
            builder.ToTable("PlaylistSong");
            builder.HasKey(ps => ps.Id);

            builder.HasOne(ps => ps.Playlist)
                .WithMany(p => p.PlaylistSongs)
                .HasForeignKey(ps => ps.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ps => ps.Song)
                .WithMany(s => s.PlaylistSongs)
                .HasForeignKey(ps => ps.SongId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class UserFavoriteSongConfiguration : IEntityTypeConfiguration<UserFavoriteSong>
    {
        public void Configure(EntityTypeBuilder<UserFavoriteSong> builder)
        {
            builder.ToTable("UserFavoriteSong");
            builder.HasKey(ufs => ufs.Id);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(ufs => ufs.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ufs => ufs.Song)
                .WithMany(s => s.FavoritedBy)
                .HasForeignKey(ufs => ufs.SongId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ListeningHistoryConfiguration : IEntityTypeConfiguration<ListeningHistory>
    {
        public void Configure(EntityTypeBuilder<ListeningHistory> builder)
        {
            builder.ToTable("ListeningHistory");
            builder.HasKey(lh => lh.Id);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(lh => lh.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(lh => lh.Song)
                .WithMany(s => s.ListeningHistories)
                .HasForeignKey(lh => lh.SongId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
