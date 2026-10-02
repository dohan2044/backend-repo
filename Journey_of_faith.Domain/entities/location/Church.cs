using Journey_of_faith.Domain.entities.masslive;
using Journey_of_faith.Domain.objectvalues.churchs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.location
{
    public class Church : AuditableEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string? Thumbnail { get; private set; }
        public string? Email { get; private set; }
        public string? Address { get; private set; }
        public int DioceseId { get; private set; }
        public string? Boss {get; private set;}
        public string? Description {get; private set;}
        public GeoLocation GeoLocation { get; private set; }
        public Diocese? Diocese { get; set; }
        private List<ChurchImage> churchImages = new List<ChurchImage>();
        private List<MassSchedule> _massSchedules = new();
        private List<LiveStream> _liveStreams = new();
        private readonly List<UserChurch> _userChurches = new();

        public IReadOnlyCollection<MassSchedule> MassSchedules => _massSchedules.AsReadOnly();
        public IReadOnlyCollection<LiveStream> LiveStreams => _liveStreams.AsReadOnly();
        public IReadOnlyCollection<UserChurch> UserChurches => _userChurches.AsReadOnly();
        public IReadOnlyCollection<ChurchImage> ChurchImages => churchImages.AsReadOnly();

        private Church() { }
        public Church(string name, string thumbnail, 
            string website, string address, int discoceId, double latitude, 
            double longtitude, Guid Userid, Guid modifier, string boss, string description
            )
        {
            if(string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Tên nhà thờ không được để trống");
            }

            if(discoceId <= 0)
            {
                throw new ArgumentOutOfRangeException("DioceseId phải là số dương");
            }
            if(string.IsNullOrEmpty(address))
            {
                throw new ArgumentNullException("Địa chỉ nhà thờ không được để trống");
            }
            Name = name;
            Thumbnail = thumbnail;
            Email = website;
            Address = address;
            DioceseId = discoceId;
            
            GeoLocation = GeoLocation.FromCoordinates(latitude, longtitude);

            CreatorUserId = Userid;
            LastModifierUserId = modifier;
            Boss = boss;
            Description = description;
        }
        public Church(int id, string name, string email, string address, int discoceId, string boss, string description, Guid lastModifier, List<MassSchedule>? massSchedules)
        {
            Id = id;
            Name = name;
            Email = email;
            Address = address;
            DioceseId = discoceId;
            Boss = boss;
            Description = description;
            LastModifierUserId = lastModifier;
            _massSchedules = massSchedules ?? new List<MassSchedule>();
        }
        public Church(int id, string name, string email, string address, int discoceId, string boss, string description, Guid lastModifier)
        {
            Id = id;
            Name = name;
            Email = email;
            Address = address;
            DioceseId = discoceId;
            Boss = boss;
            Description = description;
            LastModifierUserId = lastModifier;
        }
        public Church(string name, string address, int discoceId, double latitude, double longtitude)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Tên nhà thờ không được để trống");
            }
            
            if (discoceId <= 0)
            {
                throw new ArgumentOutOfRangeException("DioceseId phải là số dương");
            }

            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentNullException("Địa chỉ nhà thờ không được để trống");
            }

            Name = name;
            Address = address;
            DioceseId = discoceId;
            GeoLocation = GeoLocation.FromCoordinates(latitude, longtitude);
        }

        public void SetLocation(double latitude, double longtitude)
        {
            GeoLocation = GeoLocation.FromCoordinates(latitude, longtitude);
        }
        public void SetImages(List<ChurchImage> images)
        {
            churchImages = images;
        }


        public void SetMassSchedule(List<MassSchedule> massSchedules)
        {
            _massSchedules = massSchedules;
        }

        public void AddMassSchedule(MassSchedule ms) => _massSchedules.Add(ms);
        public void AddLiveStream(LiveStream ls) => _liveStreams.Add(ls);
    }
}
