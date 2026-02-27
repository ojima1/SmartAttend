using SmartAttend.Domain.Common;

namespace SmartAttend.Domain.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;

        // Navigation property for the schedule
        public virtual WorkSchedule? DefaultSchedule { get; set; }

        private Department() { }

        public static Department Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Department name cannot be empty.");

            return new Department
            {
                Id = Guid.NewGuid(),
                Name = name
            };
        }
    }
}