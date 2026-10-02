using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.churchs.dtos
{
    public class CreateDiscoseDto
    {
        public required string Name { get; set; }
        public string? Website { get; set; }
        public required string Address { get; set; }
        public string? Thumbnail { get; set; }
        public Guid CreatorUserId { get; set; }
        public Guid LastModifierUserId { get; set; }
    }


    public class DioceseView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }
        public string Thumbnail { get; set; }
        public Guid CreatorUserId { get; set; }
        public string CreationTime { get; set; }
        public Guid LastModifierUserId { get; set; }
        public string LastModificationTime { get; set; }

        public List<ChurchView> ChurchViews { get; set;  } = new List<ChurchView>();
    }
}
