using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.musics
{
    public class Song : AuditableEntity
    {
        public string Title { get; private set; } = string.Empty;
        public int ArtistId { get; private set; }
        public int? AlbumId { get; private set; }
        public int? Duration { get; private set; }
        public string? AudioUrl { get; private set; }
        public string? CoverImageUrl { get; private set; }
        public string? Lyric { get; private set; }
        public int? PlayCount { get; private set; }
        public bool? IsActive { get; private set; }
        public Artist Artist { get; set; } = null!;
        public Album? Album { get; set; }

        private readonly List<ListeningHistory> _listeningHistories = new();
        private readonly List<PlaylistSong> _playlistSongs = new();
        private readonly List<UserFavoriteSong> _userFavoriteSongs = new();
        private readonly List<SongCategoryMapping> _categoryMappings = new();

        public IReadOnlyCollection<ListeningHistory> ListeningHistories => _listeningHistories.AsReadOnly();
        public IReadOnlyCollection<PlaylistSong> PlaylistSongs => _playlistSongs.AsReadOnly();
        public IReadOnlyCollection<UserFavoriteSong> FavoritedBy => _userFavoriteSongs.AsReadOnly();
        public IReadOnlyCollection<SongCategoryMapping> CategoryMappings => _categoryMappings.AsReadOnly();

        public Song() { }

        public Song(string title, int artistId, int? albumId, int duration, string? audio,
        string? converImageUrl, string lyric, int playCount, bool isActive)
        {
            Title = title;
            ArtistId = artistId;
            AlbumId = albumId;
            Duration = duration;
            AudioUrl = audio;
            CoverImageUrl = converImageUrl;
            Lyric = lyric;
            PlayCount = playCount;
            IsActive = isActive;
        }

    }
}
