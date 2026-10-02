using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.musics
{
    public class Artist : AuditableEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string? ImageUrl { get; private set; }
        public ICollection<Album> Albums { get; set; } = [];
        public ICollection<Song> Songs { get; set; } = [];


        public Artist() {}

        public Artist(string name, string description, string? imageUrl)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Name artist is reuirec.");

            if(string.IsNullOrEmpty(description))
                throw new ArgumentNullException("Must have one line description.");

            Name = name;
            Description = description;
            ImageUrl = imageUrl;
        }

        public void UpdateImage(string imageUrl)
        {
            if(string.IsNullOrEmpty(imageUrl))
            {
               throw new ArgumentNullException("Hình ảnh không hợp lệ."); 
            }

            ImageUrl = imageUrl;
        }
    }
}
