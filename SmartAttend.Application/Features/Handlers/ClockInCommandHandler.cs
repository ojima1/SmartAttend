using MediatR;
using SmartAttend.Application.DTOs;
using SmartAttend.Application.Interfaces;
using SmartAttend.Application.Features.Attendance.Commands;
using SmartAttend.Domain.Entities;
using SmartAttend.Domain.Enums;
using SmartAttend.Domain.ValueObjects;

namespace SmartAttend.Application.Features.Handlers;

public class ClockInCommandHandler : IRequestHandler<ClockInCommand, ClockInResponse>
{
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IAttendanceRepository _attendanceRepo;
    private readonly IFaceVerificationService _faceService;
    private readonly IGeolocationService _geoService;
    private readonly IVpnDetectionService _vpnService;
    private readonly IScheduleService _scheduleService; // Added this
    private readonly IUnitOfWork _uow;

    public ClockInCommandHandler(
        IEmployeeRepository employeeRepo,
        IAttendanceRepository attendanceRepo,
        IFaceVerificationService faceService,
        IGeolocationService geoService,
        IVpnDetectionService vpnService,
        IScheduleService scheduleService,
        IUnitOfWork uow)
    {
        _employeeRepo = employeeRepo;
        _attendanceRepo = attendanceRepo;
        _faceService = faceService;
        _geoService = geoService;
        _vpnService = vpnService;
        _scheduleService = scheduleService;
        _uow = uow;
    }

    public async Task<ClockInResponse> Handle(ClockInCommand request, CancellationToken ct)
    {
        // 1. Fetch Employee
        var employee = await _employeeRepo.GetByEmailAsync(request.Email, ct);
        if (employee == null)
            return new ClockInResponse(false, "Employee not found", null);

        // 2. Check for duplicate clock-in (using WAT date to be safe)
        var todayWat = DateOnly.FromDateTime(WatTime.FromUtc(DateTime.UtcNow));
        if (await _attendanceRepo.ExistsForDateAsync(employee.Id, todayWat, ct))
            return new ClockInResponse(false, "Already clocked in today", null);

        // 3. Security Checks (VPN & Geo) - Skip if Remote
        if (employee.Role != EmployeeRole.Remote)
        {
            var vpn = await _vpnService.CheckAsync(request.IpAddress, ct);
            if (vpn.IsVpn)
                return new ClockInResponse(false, "VPN detected. Use office network.", null);

            var geo = await _geoService.ValidateAsync(request.Location, ct);
            if (!geo.IsWithinRange)
                return new ClockInResponse(false, $"Outside range ({geo.DistanceMetres}m)", null);
        }

        // 4. Biometrics (Face & Liveness)
        var face = await _faceService.VerifyCombinedAsync(employee.FaceImageBase64, request.FaceBase64, ct);
        if (!face.LivenessResult.IsLive || !face.FaceMatchResult.IsMatch)
            return new ClockInResponse(false, "Biometric verification failed", null);

        // 5. Get DayType (WorkDay/Holiday/etc) from Schedule Service
        // 5. Get Schedule from Service
        var schedule = await _scheduleService.GetTodayScheduleAsync(employee.Id, ct);

        // Determine if today is a valid work day based on the 7-day schedule
        bool isWorkDay = DateTime.UtcNow.DayOfWeek switch
        {
            DayOfWeek.Monday => schedule.IsMondayEnabled,
            DayOfWeek.Tuesday => schedule.IsTuesdayEnabled,
            DayOfWeek.Wednesday => schedule.IsWednesdayEnabled,
            DayOfWeek.Thursday => schedule.IsThursdayEnabled,
            DayOfWeek.Friday => schedule.IsFridayEnabled,
            DayOfWeek.Saturday => schedule.IsSaturdayEnabled,
            DayOfWeek.Sunday => schedule.IsSundayEnabled,
            _ => false
        };

        // 6. Create Record
        // Use the calculated isWorkDay to set the DayType
        var record = AttendanceRecord.Create(
            employee,
            DateTime.UtcNow,
            isWorkDay ? DayType.WorkDay : DayType.NonWorkDay, // Use logic here
            request.Location);

        _attendanceRepo.Add(record);
        await _uow.SaveChangesAsync(ct);

        // 7. Generate TTS
        string timeStr = WatTime.GetTimeOfDay(DateTime.UtcNow).ToString("HH:mm");
        string tts = record.Status == AttendanceStatus.OnTime
            ? $"Good morning {employee.FullName}. Clock-in successful at {timeStr} WAT. You are on time."
            : $"Good morning {employee.FullName}. Clock-in recorded at {timeStr} WAT. You are late.";

        return new ClockInResponse(true, tts, record.Status.ToString());
    }
}