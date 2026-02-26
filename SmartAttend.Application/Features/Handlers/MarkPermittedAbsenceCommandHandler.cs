using MediatR;
using SmartAttend.Application.Features.Attendance.Commands;
using SmartAttend.Application.Interfaces;
using SmartAttend.Domain.Entities;
using SmartAttend.Domain.Enums;

namespace SmartAttend.Application.Features.Handlers;

public class MarkPermittedAbsenceCommandHandler : IRequestHandler<MarkPermittedAbsenceCommand, MarkPermittedAbsenceResponse>
{
    private readonly IAttendanceRepository _attendanceRepo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IUnitOfWork _uow;

    public MarkPermittedAbsenceCommandHandler(IAttendanceRepository attendanceRepo, IEmployeeRepository employeeRepo, IUnitOfWork uow)
    {
        _attendanceRepo = attendanceRepo;
        _employeeRepo = employeeRepo;
        _uow = uow;
    }

    public async Task<MarkPermittedAbsenceResponse> Handle(MarkPermittedAbsenceCommand request, CancellationToken ct)
    {
        // 1. Validate the Authorizer (The person approving)
        var authorizer = await _employeeRepo.GetByIdAsync(request.ManagerId, ct);

        if (authorizer == null)
            return new MarkPermittedAbsenceResponse(false, "Authorizer not found.");

        // 2. Strict Role Check
        bool isAuthorized = authorizer.Role == EmployeeRole.Manager ||
                            authorizer.Role == EmployeeRole.HR ||
                            authorizer.Role == EmployeeRole.Admin;

        if (!isAuthorized)
        {
            return new MarkPermittedAbsenceResponse(false, "Unauthorized: Managerial role required.");
        }

        // 3. Get or Create the Attendance Record for that day
        var record = await _attendanceRepo.GetByEmployeeAndDateAsync(request.EmployeeId, request.Date, ct);

        if (record == null)
        {
            // If the employee didn't clock in at all, we create an "Absence" record to mark it as permitted
            record = AttendanceRecord.CreateAbsence(request.EmployeeId, request.Date);
            _attendanceRepo.Add(record);
        }

        // 4. Update the record using the Domain Method
        record.MarkAsPermitted(request.ManagerId, request.Note);

        // 5. Persist to Database
        await _uow.SaveChangesAsync(ct);

        return new MarkPermittedAbsenceResponse(true, "Absence marked as permitted.");
    }
}