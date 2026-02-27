using SmartAttend.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Domain.Entities
{
    public class WorkSchedule : BaseEntity
    {
        public Guid? EmployeeId { get; private set; }
        public Guid? DepartmentId { get; private set; }

        // Business Rule: 3 Office, 2 Remote
        public int WeeklyOfficeDays { get; private set; } = 3;
        public int WeeklyRemoteDays { get; private set; } = 2;

        // Day-by-Day toggles (Matched to ScheduleService)
        public bool IsMondayEnabled { get; set; } = true;
        public bool IsTuesdayEnabled { get; set; } = true;
        public bool IsWednesdayEnabled { get; set; } = true;
        public bool IsThursdayEnabled { get; set; } = true;
        public bool IsFridayEnabled { get; set; } = true;
        public bool IsSaturdayEnabled { get; set; } = false;
        public bool IsSundayEnabled { get; set; } = false;

        // EF Core constructor
        private WorkSchedule() { }

        public static WorkSchedule CreateForEmployee(Guid employeeId) =>
            new() { Id = Guid.NewGuid(), EmployeeId = employeeId };

        public static WorkSchedule CreateForDepartment(Guid departmentId) =>
            new() { Id = Guid.NewGuid(), DepartmentId = departmentId };
    }
}