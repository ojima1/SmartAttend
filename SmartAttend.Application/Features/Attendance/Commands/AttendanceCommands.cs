using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Application.Features.Attendance.Commands
{

    public record ClockInCommand(string Email, string FaceBase64, GeoCoordinate Location, string IpAddress) : IRequest<ClockInResponse>;
    public record ClockInResponse(bool Success, string Message, string? Status);

    public record ClockOutCommand(Guid EmployeeId, GeoCoordinate Location) : IRequest<ClockOutResponse>;
    public record ClockOutResponse(bool Success, string Message, bool IsEarlyDeparture);

    public record MarkPermittedAbsenceCommand(Guid EmployeeId, DateOnly Date, string Note, Guid ManagerId) : IRequest<MarkPermittedAbsenceResponse>;
    public record MarkPermittedAbsenceResponse(bool Success, string Message);


    public record AssignScheduleCommand(Guid EmployeeId, List<SmartAttend.Domain.ValueObjects.WorkSchedule> Schedules)
    : IRequest<AssignScheduleResponse>;

    public record AssignScheduleResponse(bool Success, string Message);
}

