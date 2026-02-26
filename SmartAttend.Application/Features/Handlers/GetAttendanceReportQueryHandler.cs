using SmartAttend.Application.Features.Attendance.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Application.Features.Handlers
{
    public class GetAttendanceReportQueryHandler : IRequestHandler<GetAttendanceReportQuery, List<AttendanceReportDto>>
    {
        private readonly IAttendanceRepository _attendanceRepo;

        public GetAttendanceReportQueryHandler(IAttendanceRepository attendanceRepo)
            => _attendanceRepo = attendanceRepo;

        public async Task<List<AttendanceReportDto>> Handle(GetAttendanceReportQuery request, CancellationToken ct)
        {
            var records = await _attendanceRepo.GetReportAsync(request.Date, request.DepartmentId, request.Status, ct);

            return records.Select(r => new AttendanceReportDto(
                r.Id,
                "Employee Name", // In the Infrastructure layer, we will join this to get the real name
                r.ClockInUtc,    // Corrected from ClockInTime
                r.ClockOutUtc,   // Corrected from ClockOutTime
                r.Status.ToString()
            )).ToList();
        }
    }
}
