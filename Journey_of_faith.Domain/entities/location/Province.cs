using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.location
{
    public class Province
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Type { get; set; }
        private List<Ward> _wards {get; set;} = new();
        public IReadOnlyList<Ward> Wards => _wards.AsReadOnly();
        public void SetWard(List<Ward> wards)
        {
            _wards = wards;
        }
    }

    public class Ward
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public int ProvinceId {get; set;}
        public string? Description {get; set;}
    }
}
