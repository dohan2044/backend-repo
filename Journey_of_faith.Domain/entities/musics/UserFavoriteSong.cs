using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.musics
{
    public class UserFavoriteSong
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int SongId { get; set; }
        public Song Song { get; set; } = null!;
        public DateTime? CreatedTime { get; set; }

        public UserFavoriteSong(Guid userId, int songId)
        {
            UserId = userId;
            SongId = songId;
            CreatedTime = DateTime.Now;
        }
    }
}
