using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Application.Interfaces
{
    public interface IFaceVerificationService
    {
        Task<CombinedBiometricResult> VerifyCombinedAsync(string referenceBase64, string liveBase64, CancellationToken ct);
    }

    public interface IVpnDetectionService
    {
        Task<VpnCheckResult> CheckAsync(string ipAddress, CancellationToken ct);
    }

    public interface IGeolocationService
    {
        Task<GeoValidationResult> ValidateAsync(SmartAttend.Domain.ValueObjects.GeoCoordinate location, CancellationToken ct);
    }
}
