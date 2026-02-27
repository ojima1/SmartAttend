using Microsoft.EntityFrameworkCore;
using SmartAttend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace SmartAttend.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<Employee>(b => {
                b.HasIndex(e => e.Email).IsUnique();
                b.Property(e => e.Email).HasMaxLength(255);
            });

            modelBuilder.Entity<AttendanceRecord>(b => {
                b.HasIndex(a => new { a.EmployeeId, a.Date });
                b.Property(a => a.Latitude).HasColumnType("decimal(9,6)");
                b.Property(a => a.Longitude).HasColumnType("decimal(9,6)");
            });
        }
    }
}
