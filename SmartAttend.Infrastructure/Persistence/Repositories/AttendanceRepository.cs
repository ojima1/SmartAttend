using Microsoft.EntityFrameworkCore;
using SmartAttend.Application.Interfaces;
using SmartAttend.Domain.Entities;
using SmartAttend.Domain.Enums;

namespace SmartAttend.Infrastructure.Persistence.Repositories;

public class AttendanceRepository(AppDbContext context) : IAttendanceRepository
{
    public async Task<AttendanceRecord?> GetByEmployeeAndDateAsync(Guid employeeId, DateOnly date, CancellationToken ct) =>
        await context.AttendanceRecords.FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == date, ct);

    public void Add(AttendanceRecord record) => context.AttendanceRecords.Add(record);
    public void Update(AttendanceRecord record) => context.AttendanceRecords.Update(record);

    // 1. Quick existence check for clock-in validation
    public async Task<bool> ExistsForDateAsync(Guid employeeId, DateOnly date, CancellationToken ct)
    {
        return await context.AttendanceRecords
            .AnyAsync(a => a.EmployeeId == employeeId && a.Date == date, ct);
    }

    // 2. Reporting logic with filtering
    public async Task<List<AttendanceRecord>> GetReportAsync(DateOnly date, Guid? deptId, AttendanceStatus? status, CancellationToken ct)
    {
        var query = context.AttendanceRecords
            .Include(a => a.Employee) // Ensure we get employee names/details
            .AsQueryable();

        query = query.Where(a => a.Date == date);

        if (deptId.HasValue)
            query = query.Where(a => a.Employee.DepartmentId == deptId.Value);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        return await query.ToListAsync(ct);
    }
}