using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartAttend.Application.Interfaces;
using SmartAttend.Infrastructure.Persistence;
using SmartAttend.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Infrastructure.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Default")));

            services.AddMemoryCache();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IGeolocationService, GeolocationService>();

            services.AddHttpClient<IFaceVerificationService, GeminiFaceVerificationService>();
            services.AddHttpClient<IVpnDetectionService, IpApiVpnDetectionService>();

            return services;
        }
    }
}
