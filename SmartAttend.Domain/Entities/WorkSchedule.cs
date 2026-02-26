using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Domain.Entities
{
    public class WorkSchedule
    {
        public Guid Id { get; private set; }
        public Guid? EmployeeId { get; private set; }
        public Guid? DepartmentId { get; private set; }

        // Business Rule: 3 Office, 2 Remote
        public int WeeklyOfficeDays { get; private set; } = 3;
        public int WeeklyRemoteDays { get; private set; } = 2;

        private WorkSchedule() { }

        public static WorkSchedule CreateForEmployee(Guid employeeId) =>
            new() { Id = Guid.NewGuid(), EmployeeId = employeeId };

        public static WorkSchedule CreateForDepartment(Guid departmentId) =>
            new() { Id = Guid.NewGuid(), DepartmentId = departmentId };
    }
}
