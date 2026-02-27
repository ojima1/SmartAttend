using Microsoft.EntityFrameworkCore;
using SmartAttend.Application.Interfaces;
using SmartAttend.Application.DTOs; // Ensure this contains ScheduleResult
using SmartAttend.Domain.Entities;
using SmartAttend.Infrastructure.Persistence;

namespace SmartAttend.Infrastructure.Services;

public class ScheduleService(AppDbContext dbContext) : IScheduleService
{
    public async Task<ScheduleResult?> GetTodayScheduleAsync(Guid employeeId, CancellationToken ct)
    {
        return await GetScheduleForDateAsync(employeeId, DateOnly.FromDateTime(DateTime.UtcNow), ct);
    }

    public async Task<ScheduleResult?> GetScheduleForDateAsync(Guid employeeId, DateOnly date, CancellationToken ct)
    {
        var employee = await dbContext.Employees
            .Include(e => e.IndividualSchedule)
            .Include(e => e.Department)
                .ThenInclude(d => d.DefaultSchedule)
            .FirstOrDefaultAsync(e => e.Id == employeeId, ct);

        if (employee == null) return null;

        // Business Rule: Individual override beats department default
        var activeSchedule = employee.IndividualSchedule ?? employee.Department?.DefaultSchedule;

        if (activeSchedule == null) return null;

        // Map Entity to DTO to satisfy the Interface Requirement
        return new ScheduleResult(
            activeSchedule.IsMondayEnabled,
            activeSchedule.IsTuesdayEnabled,
            activeSchedule.IsWednesdayEnabled,
            activeSchedule.IsThursdayEnabled,
            activeSchedule.IsFridayEnabled,
            activeSchedule.IsSaturdayEnabled,
            activeSchedule.IsSundayEnabled
        );
    }

    public bool IsWorkDay(Employee employee, DateTime date)
    {
        var schedule = employee.IndividualSchedule ?? employee.Department?.DefaultSchedule;
        if (schedule == null) return false;

        return date.DayOfWeek switch
        {
            DayOfWeek.Monday => schedule.IsMondayEnabled,
            DayOfWeek.Tuesday => schedule.IsTuesdayEnabled,
            DayOfWeek.Wednesday => schedule.IsWednesdayEnabled,
            DayOfWeek.Thursday => schedule.IsThursdayEnabled,
            DayOfWeek.Friday => schedule.IsFridayEnabled,
            DayOfWeek.Saturday => schedule.IsSaturdayEnabled,
            DayOfWeek.Sunday => schedule.IsSundayEnabled,
            _ => false
        };
    }
}