using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Application.Interfaces
{
    public interface IScheduleService
    {
        Task<ScheduleResult> GetTodayScheduleAsync(Guid employeeId, CancellationToken ct);
        Task<ScheduleResult> GetScheduleForDateAsync(Guid employeeId, DateOnly date, CancellationToken ct);
    }

    public interface IAttendanceStatusResolver
    {
        AttendanceStatus Resolve(EmployeeRole role, DateTime clockInUtc);
    }
}
