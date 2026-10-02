using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.musics
{
    public class Playlist
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime? CreatedTime { get; set; }

        private readonly List<PlaylistSong> _playlistSongs = new();

        public IReadOnlyCollection<PlaylistSong> PlaylistSongs => _playlistSongs.AsReadOnly();

        public void AddPlaylistSong(PlaylistSong ps) => _playlistSongs.Add(ps);
    }
}
