using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.musics
{
    public class ListeningHistory
    {
        public int Id { get;private set; }
        public Guid UserId { get; private set; }
        public int SongId { get; private set; }
        public DateTime? ListenTime { get; private set; }
        public Song Song { get; set; } = null!;
        public ListeningHistory(Guid userId, int songId)
        {
            UserId = userId;
            SongId = songId;
            ListenTime = DateTime.UtcNow;
        }
    }
}
