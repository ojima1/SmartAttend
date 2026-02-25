using SmartAttend.Domain.Enums;
using SmartAttend.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Domain.Entities
{
    public class AttendanceRecord
    {
        public Guid Id { get; private set; }
        public Guid EmployeeId { get; private set; }
        public DateTime? ClockInUtc { get; private set; }
        public DateTime? ClockOutUtc { get; private set; }
        public DayType DayType { get; private set; }
        public AttendanceStatus Status { get; private set; }
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public bool FaceVerified { get; private set; }
        public bool IsEarlyDeparture { get; private set; }
        public DateOnly Date { get; private set; }
        public Guid? PermittedByManagerId { get; private set; }
        public string? PermissionNote { get; private set; }

        private AttendanceRecord() { }

        public static AttendanceRecord Create(Employee emp, DateTime clockInUtc, DayType dayType, GeoCoordinate? coords)
        {
            var watTime = WatTime.GetTimeOfDay(clockInUtc);
            var lateThreshold = emp.Role == EmployeeRole.Intern ? new TimeOnly(8, 0) : new TimeOnly(9, 0);

            return new AttendanceRecord
            {
                Id = Guid.NewGuid(),
                EmployeeId = emp.Id,
                ClockInUtc = clockInUtc,
                DayType = dayType,
                Date = DateOnly.FromDateTime(WatTime.FromUtc(clockInUtc)),
                Status = watTime <= lateThreshold ? AttendanceStatus.OnTime : AttendanceStatus.Late,
                Latitude = coords?.Latitude,
                Longitude = coords?.Longitude
            };
        }

        public static AttendanceRecord CreateAbsence(Guid empId, DateOnly date) =>
            new() { Id = Guid.NewGuid(), EmployeeId = empId, Date = date, Status = AttendanceStatus.Absent };

        public void ClockOut(DateTime utcNow)
        {
            ClockOutUtc = utcNow;
            var watTime = WatTime.GetTimeOfDay(utcNow);
            IsEarlyDeparture = watTime < new TimeOnly(16, 0);
        }

        public void MarkAsPermitted(Guid managerId, string note)
        {
            Status = AttendanceStatus.PermittedAbsent;
            PermittedByManagerId = managerId;
            PermissionNote = note;
        }
    }
}
