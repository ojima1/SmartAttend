using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Domain.ValueObjects
{
    public record GeoCoordinate(double Latitude, double Longitude)
    {
        public double DistanceTo(GeoCoordinate other)
        {
            const double R = 6371000; // Earth radius in metres
            var dLat = ToRadians(other.Latitude - Latitude);
            var dLon = ToRadians(other.Longitude - Longitude);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double angle) => Math.PI * angle / 180.0;
    }
}
