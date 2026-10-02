using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.churchs.dtos
{
    public class CreateChurchDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Thumbnail { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int DioceseId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public Guid CreatorUserId { get; set; }
        public Guid LastModifierUserId { get; set; }
        public string? Boss {get; set;}
        public string? Description {get; set;}
    }

    public class ChurchView
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Thumbnail { get; set; }
        public string? Website { get; set; }
        public string? Address { get; set; }
        public int DioceseId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }


    public class ChurchUpdateDto
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Thumbnail { get; set; }
        public string? Email { get; set; }
        public string ?Address { get; set; }
        public int? DioceseId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
