using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.musics
{
    public class SongCategoryMapping
    {
        public int Id { get; set; }
        public int SongId { get; set; }
        public int CategoryId { get; set; }
        public Song Song { get; set; } = null!;
        public SongCategory Category { get; set; } = null!;

        public SongCategoryMapping() {}
        public SongCategoryMapping(int songId, int categoryId)
        {
            if(songId <= 0)
            {
                throw new ArgumentException(nameof(songId), "SongId is invalid.");
            }
            if(categoryId <= 0)
            {
                throw new ArgumentException(nameof(categoryId), "categoryId is invalid.");
            }

            SongId = songId;
            CategoryId = categoryId;
        }
    }
}
