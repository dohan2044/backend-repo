using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.musics
{
    public class SongCategory : AuditableEntity
    {
        public string? Name { get; private set; }
        public ICollection<SongCategoryMapping> SongMappings { get; set; } = [];

        public SongCategory() {}

        public SongCategory(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is required.");
            } 

            Name = name;
        }
    }
}
