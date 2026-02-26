using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Application.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<AttendanceRecord?> GetByEmployeeAndDateAsync(Guid employeeId, DateOnly date, CancellationToken ct);
        Task<bool> ExistsForDateAsync(Guid employeeId, DateOnly date, CancellationToken ct);
        void Add(AttendanceRecord record);
        Task<List<AttendanceRecord>> GetReportAsync(DateOnly date, Guid? deptId, AttendanceStatus? status, CancellationToken ct);
    }

    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Employee?> GetByEmailAsync(string email, CancellationToken ct);
        void Add(Employee employee);
    }

    public interface IUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken ct);
    }
}
