using Xunit;
using Moq;
using FluentAssertions;
using SmartAttend.Application.DTOs;
using SmartAttend.Application.Features.Attendance.Commands;
using SmartAttend.Application.Features.Handlers;
using SmartAttend.Application.Interfaces;
using SmartAttend.Domain.Entities;
using SmartAttend.Domain.Enums;
using SmartAttend.Domain.ValueObjects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAttend.UnitTests.Application
{
    public class ClockInHandlerTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepo = new();
        private readonly Mock<IAttendanceRepository> _attendanceRepo = new();
        private readonly Mock<IFaceVerificationService> _faceService = new();
        private readonly Mock<IGeolocationService> _geoService = new();
        private readonly Mock<IVpnDetectionService> _vpnService = new();
        private readonly Mock<IScheduleService> _scheduleService = new();
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly ClockInCommandHandler _handler;

        public ClockInHandlerTests()
        {
            _handler = new ClockInCommandHandler(
                _employeeRepo.Object,
                _attendanceRepo.Object,
                _faceService.Object,
                _geoService.Object,
                _vpnService.Object,
                _scheduleService.Object,
                _uow.Object);
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenVPNIsDetectedForOfficeStaff()
        {
            // Arrange
            var command = new ClockInCommand("john@work.com", "face_data", new GeoCoordinate(6.5, 3.3), "1.1.1.1");
            var employee = Employee.Create("John Doe", "john@work.com", "hash", EmployeeRole.Regular, Guid.NewGuid());

            _employeeRepo.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(employee);

            _vpnService.Setup(x => x.CheckAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new VpnCheckResult(true, "Proxy Detected"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("VPN detected");
        }

        [Fact]
        public async Task Handle_ShouldSkipVPN_WhenEmployeeIsRemote()
        {
            // Arrange
            var command = new ClockInCommand("remote@work.com", "face_data", new GeoCoordinate(0, 0), "1.1.1.1");
            var employee = Employee.Create("Remote User", "remote@work.com", "hash", EmployeeRole.Remote, Guid.NewGuid());
            employee.RegisterFace("existing_face");

            _employeeRepo.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(employee);

            // Setup Biometrics - Swapped Liveness and FaceMatch to match DTO constructor expectations
            _faceService.Setup(x => x.VerifyCombinedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new CombinedBiometricResult(
                            new LivenessResult(true, 1.0, "Live"),
                            new FaceMatchResult(true, 1.0, "Matched")));

            _scheduleService.Setup(x => x.GetTodayScheduleAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                            .ReturnsAsync(new ScheduleResult(DayType.WorkDay));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();

            // Fixed lowercase 'it' to 'It'
            _vpnService.Verify(x => x.CheckAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}