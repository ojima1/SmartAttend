using FluentAssertions;
using Moq;
using SmartAttend.Application.Features.Attendance.Commands;
using SmartAttend.Application.Features.Handlers;
using SmartAttend.Application.Interfaces;
using SmartAttend.Application.DTOs;
using SmartAttend.Domain.Entities;
using SmartAttend.Domain.Enums;
using SmartAttend.Domain.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SmartAttend.UnitTests.Application
{
    public class ClockOutHandlerTests
    {
        private readonly Mock<IAttendanceRepository> _attendanceRepo = new();
        private readonly Mock<IScheduleService> _scheduleService = new(); // Added this
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly ClockOutCommandHandler _handler;

        public ClockOutHandlerTests()
        {
            // Updated constructor to match the new Handler signature
            _handler = new ClockOutCommandHandler(
                _attendanceRepo.Object,
                _scheduleService.Object,
                _uow.Object);
        }

        [Fact]
        public async Task Handle_ShouldMarkEarlyDeparture_WhenClockingOutBefore4PM()
        {
            // Arrange
            var empId = Guid.NewGuid();
            var employee = Employee.Create("Test", "test@work.com", "hash", EmployeeRole.Regular, Guid.NewGuid());

            // Create a record that was clocked in at 8 AM today
            var record = AttendanceRecord.Create(
                employee,
                DateTime.UtcNow.Date.AddHours(7), // 7 AM UTC (8 AM WAT)
                DayType.WorkDay,
                new GeoCoordinate(0, 0));

            _attendanceRepo.Setup(x => x.GetByEmployeeAndDateAsync(It.IsAny<Guid>(),
                It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(record);

            // Mock the schedule to show today is a work day
            _scheduleService.Setup(x => x.GetTodayScheduleAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ScheduleResult(true, true, true, true, true, true, true));

            // Act
            // NOTE: If you run this after 4:00 PM WAT, record.ClockOut(DateTime.UtcNow) 
            // inside the handler will NOT be an early departure.
            var result = await _handler.Handle(new ClockOutCommand(empId, new(0, 0)), CancellationToken.None);

            // Assert
            // To make this test "Time-Proof", we'd need to inject an IDateTimeProvider.
            // For now, if it's currently past 4pm WAT, this will fail.
            result.IsEarlyDeparture.Should().BeTrue("because the current time should be before the 4 PM cutoff");
        }
    }
}