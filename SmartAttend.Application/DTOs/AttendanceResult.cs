using SmartAttend.Domain.Enums;

namespace SmartAttend.Application.DTOs;

public record LivenessResult(bool IsLive, double Confidence, string Reason);
public record FaceMatchResult(bool IsMatch, double Confidence, string Reason);
public record CombinedBiometricResult(LivenessResult LivenessResult, FaceMatchResult FaceMatchResult);
public record VpnCheckResult(bool IsVpn, string Source);
public record GeoValidationResult(bool IsWithinRange, double DistanceMetres);
public record ScheduleResult(DayType DayType);

// Reporting DTOs
public record AttendanceReportDto(Guid Id, string EmployeeName, DateTime? ClockIn, DateTime? ClockOut, string Status);
public record LateArrivalDto(string EmployeeName, DateTime ClockIn, string Department);
public record EarlyDepartureDto(string EmployeeName, DateTime ClockOut, string Department);