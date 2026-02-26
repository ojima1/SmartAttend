using Xunit;
using Moq;
using FluentAssertions;
using SmartAttend.Application.Features.Attendance.Commands;
using SmartAttend.Application.Features.Handlers;
using SmartAttend.Application.Interfaces;
using SmartAttend.Domain.Entities;
using SmartAttend.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAttend.UnitTests.Application
{
    public class AbsenceHandlerTests
    {
        private readonly Mock<IAttendanceRepository> _attendanceRepo = new();
        private readonly Mock<IEmployeeRepository> _employeeRepo = new();
        private readonly Mock<IUnitOfWork> _uow = new();

        [Fact]
        public async Task Handle_ShouldFail_WhenNonManagerTriesToPermitAbsence()
        {
            // Arrange
            var actingId = Guid.NewGuid();

            // We setup the mock to return NULL. 
            // If the handler is working, it MUST return Success = false.
            _employeeRepo.Setup(x => x.GetByIdAsync(actingId, It.IsAny<CancellationToken>()))
                         .ReturnsAsync((Employee)null!);

            var handler = new MarkPermittedAbsenceCommandHandler(_attendanceRepo.Object, _employeeRepo.Object, _uow.Object);

            // Act
            var command = new MarkPermittedAbsenceCommand(Guid.NewGuid(), new DateOnly(2026, 1, 1), "Note", actingId);
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse("because the authorizer was not found in the database");
        }
    }
}