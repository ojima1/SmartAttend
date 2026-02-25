using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Domain.ValueObjects
{
    public static class WatTime
    {
        private static readonly TimeZoneInfo WatZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");

        public static DateTime FromUtc(DateTime utc) => TimeZoneInfo.ConvertTimeFromUtc(utc, WatZone);

        public static TimeOnly GetTimeOfDay(DateTime utc) => TimeOnly.FromDateTime(FromUtc(utc));
    }
}
