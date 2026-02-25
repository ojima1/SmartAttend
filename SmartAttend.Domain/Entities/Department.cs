using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Domain.Entities
{
    public class Department
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        // Private constructor ensures creation only via the Factory Method
        private Department() { }

        /// <summary>
        /// Factory method to create a new Department
        /// </summary>
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
