using FluentAssertions;
using Moq;
using SmartAttend.Application.Features.Attendance.Commands;
using SmartAttend.Application.Features.Handlers;
using SmartAttend.Application.Interfaces;
using SmartAttend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.UnitTests.Application
{
    public class ClockOutHandlerTests
    {
        private readonly Mock<IAttendanceRepository> _attendanceRepo = new();
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly ClockOutCommandHandler _handler;

        public ClockOutHandlerTests() => _handler = new ClockOutCommandHandler(_attendanceRepo.Object, _uow.Object);

        [Fact]
        public async Task Handle_ShouldMarkEarlyDeparture_WhenClockingOutBefore4PM()
        {
            // Arrange
            var empId = Guid.NewGuid();
            var record = AttendanceRecord.CreateAbsence(empId, DateOnly.FromDateTime(DateTime.UtcNow));
            _attendanceRepo.Setup(x => x.GetByEmployeeAndDateAsync(It.IsAny<Guid>(), 
                It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(record);

            // Act
            // Use a fixed time for testing (e.g., 2 PM WAT)
            var result = await _handler.Handle(new ClockOutCommand(empId, new(0, 0)), CancellationToken.None);

            // Assert
            result.IsEarlyDeparture.Should().BeTrue();
            result.Message.Should().Contain("Early departure");
        }
    }
}
