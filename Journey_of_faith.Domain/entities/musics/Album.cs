using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.musics
{
    public class Album: AuditableEntity {

        public string Title { get; set; } = string.Empty;
        public int ArtistId { get; set; }
        public int? ReleaseYear { get; set; }
        public string? CoverImageUrl { get; set; }
        public Artist Artist { get; set; } = null!;
        public ICollection<Song> Songs { get; set; } = [];

        public Album() {}

        public Album(string title, int artistId)
        {
            if(string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException("Title is not null and not empty.");
            }

            if(artistId <= 0)
            {
                throw new ArgumentException(nameof(artistId), "artistId is invalid.");
            }

            Title = title;
            ArtistId = artistId;
        }

        public Album(string title, int artistId, int releaseYear)
        {
             if(string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException("Title is not null and not empty.");
            }

            if(artistId <= 0)
            {
                throw new ArgumentException(nameof(artistId), "artistId is invalid.");
            }

            if(releaseYear <= 0 || releaseYear <= 2000 || releaseYear > 2026)
            {
                throw new ArgumentException("ReleaseYear is invalid.");
            }

            Title = title;
            ArtistId = artistId;
            ReleaseYear = releaseYear;
        }

        public Album(string title, int artistId, int releaseYear, string coverImageUrl)
        {
             if(string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title),"Title is not null and not empty.");
            }

            if(artistId <= 0)
            {
                throw new ArgumentException(nameof(artistId), "artistId is invalid.");
            }

            if(releaseYear <= 0 || releaseYear <= 2000 || releaseYear > 2026)
            {
                throw new ArgumentException(nameof(releaseYear),"ReleaseYear is invalid.");
            }
            if(string.IsNullOrEmpty(coverImageUrl))
            {
                throw new ArgumentNullException(nameof(coverImageUrl), "Image is not null or not empty.");
            }
            Title = title;
            ArtistId = artistId;
            ReleaseYear = releaseYear;
        }


    }
}
