using SmartAttend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Domain.Entities
{
    public class Employee
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public EmployeeRole Role { get; private set; }
        public Guid DepartmentId { get; private set; }
        public Department Department { get; private set; }
        public string? FaceImageBase64 { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

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
