using Microsoft.Extensions.Configuration;
using SmartAttend.Application.DTOs;
using SmartAttend.Application.Interfaces;
using SmartAttend.Domain.ValueObjects;

namespace SmartAttend.Infrastructure.Services;

public class GeolocationService(IConfiguration config) : IGeolocationService
{
    public async Task<GeoValidationResult> ValidateAsync(GeoCoordinate userCoords, CancellationToken ct)
    {
        var officeLat = config.GetValue<double>("Office:Latitude");
        var officeLng = config.GetValue<double>("Office:Longitude");
        var allowedRadius = config.GetValue<double>("Office:AllowedRadiusMetres");

        // Haversine formula calculation
        var d1 = userCoords.Latitude * (Math.PI / 180.0);
        var num1 = userCoords.Longitude * (Math.PI / 180.0);
        var d2 = officeLat * (Math.PI / 180.0);
        var num2 = officeLng * (Math.PI / 180.0) - num1;
        var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                 Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

        var distance = 6371000.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));

        bool isWithinRange = distance <= allowedRadius;

        // Passing only 2 arguments to match the constructor: (bool, double)
        return new GeoValidationResult(isWithinRange, distance);
    }
}