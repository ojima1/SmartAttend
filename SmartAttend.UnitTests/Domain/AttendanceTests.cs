using SmartAttend.Domain.Entities;
using SmartAttend.Domain.Enums;
using SmartAttend.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.UnitTests.Domain
{
    public class AttendanceTests
    {
        [Fact]
        public void GeoCoordinate_Distance_Between_Lagos_Points_Is_Accurate()
        {
            // Ikeja City Mall to Maryland Mall (~4.5km)
            var mallA = new GeoCoordinate(6.6120, 3.3451);
            var mallB = new GeoCoordinate(6.5722, 3.3653);

            var distance = mallA.DistanceTo(mallB);

            Assert.InRange(distance, 4000, 5000);
        }

        [Fact]
        public void ClockOut_Before_1600_WAT_Sets_EarlyDeparture_True()
        {
            var record = AttendanceRecord.CreateAbsence(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now));
            // 14:00 WAT is 13:00 UTC
            var earlyTime = new DateTime(2024, 1, 1, 13, 0, 0, DateTimeKind.Utc);

            record.ClockOut(earlyTime);

            Assert.True(record.IsEarlyDeparture);
        }

        [Fact]
        public void MarkAsPermitted_Updates_Status_And_Manager()
        {
            var record = AttendanceRecord.CreateAbsence(Guid.NewGuid(), new DateOnly(2024, 1, 1));
            var managerId = Guid.NewGuid();

            record.MarkAsPermitted(managerId, "Sick leave approved");

            Assert.Equal(AttendanceStatus.PermittedAbsent, record.Status);
            Assert.Equal(managerId, record.PermittedByManagerId);
        }
    }
}
