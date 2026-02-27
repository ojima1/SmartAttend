using Microsoft.EntityFrameworkCore;
using SmartAttend.Application.Interfaces;
using SmartAttend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Infrastructure.Persistence.Repositories
{

    public class EmployeeRepository(AppDbContext context) : IEmployeeRepository
    {
        public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct) =>
            await context.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);

        public async Task<Employee?> GetByEmailAsync(string email, CancellationToken ct) =>
            await context.Employees.FirstOrDefaultAsync(e => e.Email == email, ct);

        public void Add(Employee employee) => context.Employees.Add(employee);
    }
}
