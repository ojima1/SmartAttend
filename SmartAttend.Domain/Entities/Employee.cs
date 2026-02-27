using SmartAttend.Domain.Enums;
using SmartAttend.Domain.Common; 

namespace SmartAttend.Domain.Entities
{
    public class Employee : BaseEntity // Inherit from BaseEntity
    {
        public string FullName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public EmployeeRole Role { get; private set; }

        public Guid DepartmentId { get; private set; }
        public virtual Department Department { get; private set; } = null!;

        // Missing Property 1: Link to personal schedule override
        public Guid? IndividualScheduleId { get; private set; }
        public virtual WorkSchedule? IndividualSchedule { get; set; }

        public string? FaceImageBase64 { get; private set; }
        public bool IsActive { get; private set; }

        private Employee() { }

        public static Employee Create(string name, string email, string hash, EmployeeRole role, Guid deptId) =>
            new()
            {
                Id = Guid.NewGuid(),
                FullName = name,
                Email = email,
                PasswordHash = hash,
                Role = role,
                DepartmentId = deptId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

        public void RegisterFace(string base64) => FaceImageBase64 = base64;
    }
}