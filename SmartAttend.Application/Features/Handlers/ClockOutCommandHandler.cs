using SmartAttend.Application.Features.Attendance.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Application.Features.Handlers
{
    public class ClockOutCommandHandler : IRequestHandler<ClockOutCommand, ClockOutResponse>
    {
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly IUnitOfWork _uow;

        public ClockOutCommandHandler(IAttendanceRepository attendanceRepo, IUnitOfWork uow)
        {
            _attendanceRepo = attendanceRepo;
            _uow = uow;
        }

        public async Task<ClockOutResponse> Handle(ClockOutCommand request, CancellationToken ct)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var record = await _attendanceRepo.GetByEmployeeAndDateAsync(request.EmployeeId, today, ct);

            if (record == null) return new ClockOutResponse(false, "No clock-in record found for today.", false);
            if (record.ClockOutUtc.HasValue) return new ClockOutResponse(false, "Already clocked out.", false);

            record.ClockOut(DateTime.UtcNow);
            await _uow.SaveChangesAsync(ct);

            string timeStr = WatTime.GetTimeOfDay(DateTime.UtcNow).ToString("HH:mm");
            string tts = record.IsEarlyDeparture
                ? $"Clock-out recorded at {timeStr} WAT. Early departure has been noted."
                : $"Good work today. Clock-out successful at {timeStr} WAT.";

            return new ClockOutResponse(true, tts, record.IsEarlyDeparture);
        }
    }
}
