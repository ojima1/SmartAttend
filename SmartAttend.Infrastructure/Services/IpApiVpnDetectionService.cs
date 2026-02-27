using Microsoft.Extensions.Caching.Memory;
using SmartAttend.Application.Interfaces;
using SmartAttend.Application.DTOs; // This must contain VpnCheckResult
using System.Net.Http.Json;
using System.Net.Http;

namespace SmartAttend.Infrastructure.Services;

public class IpApiVpnDetectionService(HttpClient http, IMemoryCache cache) : IVpnDetectionService
{
    public async Task<VpnCheckResult> CheckAsync(string ip, CancellationToken ct)
    {
        // Check Cache
        if (cache.TryGetValue(ip, out VpnCheckResult? cachedResult) && cachedResult != null)
            return cachedResult;

        try
        {
            var response = await http.GetFromJsonAsync<IpApiResponse>(
                $"http://ip-api.com/json/{ip}?fields=proxy,hosting", ct);

            bool isVpn = response?.Proxy == true || response?.Hosting == true;

            var result = new VpnCheckResult(isVpn, isVpn ? "VPN/Proxy Detected" : "Clean Connection");

            // Cache for 10 minutes
            cache.Set(ip, result, TimeSpan.FromMinutes(10));

            return result;
        }
        catch
        {
            // Fail-safe: allow access if the detection API is down
            return new VpnCheckResult(false, "VPN check bypassed due to service error");
        }
    }
}

public record IpApiResponse(bool Proxy, bool Hosting);