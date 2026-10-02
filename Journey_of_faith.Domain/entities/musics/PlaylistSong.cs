using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.musics
{
    public class PlaylistSong
    {
        public int Id { get; set; }
        public int PlaylistId { get; set; }
        public int SongId { get; set; }
        public int? OrderIndex { get; set; }
        public Playlist Playlist { get; set; } = null!;
        public Song Song { get; set; } = null!;
    }
}
