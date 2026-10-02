using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.objectvalues.churchs
{
    public class GeoLocation
    {
        public double? Latitude { get; }
        public double? Longitude { get; }

        public GeoLocation(double? latitude, double? longitude)
        {
            Latitude = latitude ?? 0;
            Longitude = longitude ?? 0;
        }

        public static GeoLocation FromCoordinates(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
            {
                throw new ArgumentOutOfRangeException(nameof(latitude), "Vĩ độ phải nằm trong khoảng -90 đến 90");
            }
            if (longitude < -180 || longitude > 180)
            {
                throw new ArgumentOutOfRangeException(nameof(longitude), "Kinh độ phải nằm trong khoảng -180 đến 180");
            }
            return new GeoLocation(latitude, longitude);
        }

    }
}
