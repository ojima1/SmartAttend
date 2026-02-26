using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Application.Features.Attendance.Queries
{

    public record GetAttendanceReportQuery(DateOnly Date, Guid? DepartmentId, AttendanceStatus? Status) : IRequest<List<AttendanceReportDto>>;
    public record GetLateArrivalsQuery(DateOnly Date, Guid? DepartmentId) : IRequest<List<LateArrivalDto>>;

    public record GetEarlyDeparturesQuery(DateOnly Date, Guid? DepartmentId)
    : IRequest<List<EarlyDepartureDto>>;
}
