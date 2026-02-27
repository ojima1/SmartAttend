using MediatR;
using SmartAttend.Application.Features.Attendance.Commands;
using SmartAttend.Application.Interfaces;
using SmartAttend.Domain.ValueObjects;

namespace SmartAttend.Application.Features.Handlers;

public class ClockOutCommandHandler : IRequestHandler<ClockOutCommand, ClockOutResponse>
{
    private readonly IAttendanceRepository _attendanceRepo;
    private readonly IScheduleService _scheduleService; // Added this
    private readonly IUnitOfWork _uow;

    public ClockOutCommandHandler(
        IAttendanceRepository attendanceRepo,
        IScheduleService scheduleService, // Added this
        IUnitOfWork uow)
    {
        _attendanceRepo = attendanceRepo;
        _scheduleService = scheduleService;
        _uow = uow;
    }

    public async Task<ClockOutResponse> Handle(ClockOutCommand request, CancellationToken ct)
    {
        // Use WAT time to ensure date consistency
        var todayWat = DateOnly.FromDateTime(WatTime.FromUtc(DateTime.UtcNow));
        var record = await _attendanceRepo.GetByEmployeeAndDateAsync(request.EmployeeId, todayWat, ct);

        if (record == null) return new ClockOutResponse(false, "No clock-in record found for today.", false);
        if (record.ClockOutUtc.HasValue) return new ClockOutResponse(false, "Already clocked out.", false);

        // Fetch schedule to pass to the domain logic if needed
        var schedule = await _scheduleService.GetTodayScheduleAsync(request.EmployeeId, ct);

        // Perform the clock out
        record.ClockOut(DateTime.UtcNow);

        await _uow.SaveChangesAsync(ct);

        string timeStr = WatTime.GetTimeOfDay(DateTime.UtcNow).ToString("HH:mm");

        // Note: record.IsEarlyDeparture is a calculated property in your Domain Entity
        string tts = record.IsEarlyDeparture
            ? $"Clock-out recorded at {timeStr} WAT. Early departure has been noted."
            : $"Good work today. Clock-out successful at {timeStr} WAT.";

        return new ClockOutResponse(true, tts, record.IsEarlyDeparture);
    }
}