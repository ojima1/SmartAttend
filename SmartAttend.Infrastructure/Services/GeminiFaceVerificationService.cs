using Microsoft.Extensions.Configuration;
using SmartAttend.Application.DTOs;
using SmartAttend.Application.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace SmartAttend.Infrastructure.Services;

public class GeminiFaceVerificationService : IFaceVerificationService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public GeminiFaceVerificationService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Gemini:ApiKey"] ?? throw new Exception("Gemini API Key missing in config.");
    }

    public async Task<CombinedBiometricResult> VerifyCombinedAsync(string refBase64, string liveBase64, CancellationToken ct)
    {
        var payload = new
        {
            contents = new[] {
                new { parts = new object[] {
                    new { text = "Compare faces in Image 1 (Reference) and Image 2 (Live). Check for liveness. Return JSON ONLY: { \"matchScore\": 0.85, \"livenessScore\": 0.90 }" },
                    new { inline_data = new { mime_type = "image/jpeg", data = refBase64 } },
                    new { inline_data = new { mime_type = "image/jpeg", data = liveBase64 } }
                }}
            }
        };

        var response = await _http.PostAsJsonAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}", payload, ct);

        if (!response.IsSuccessStatusCode)
        {
            return new CombinedBiometricResult(
                new LivenessResult(false, 0, "Service error"),
                new FaceMatchResult(false, 0, "Service error")
            );
        }

        var rawResponse = await response.Content.ReadAsStringAsync(ct);

        try
        {
            using var doc = JsonDocument.Parse(rawResponse);
            var jsonText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "";
            var cleanJson = jsonText.Replace("```json", "").Replace("```", "").Trim();
            var scores = JsonSerializer.Deserialize<GeminiScoreResponse>(cleanJson);

            // Create the specific Result objects required by your DTO
            var liveness = new LivenessResult(scores!.LivenessScore >= 0.75, (float)scores.LivenessScore, "Liveness check complete");
            var faceMatch = new FaceMatchResult(scores.MatchScore >= 0.80, (float)scores.MatchScore, "Face match check complete");

            return new CombinedBiometricResult(liveness, faceMatch);
        }
        catch
        {
            return new CombinedBiometricResult(
                new LivenessResult(false, 0, "Parsing error"),
                new FaceMatchResult(false, 0, "Parsing error")
            );
        }
    }

    private record GeminiScoreResponse(double MatchScore, double LivenessScore);
}