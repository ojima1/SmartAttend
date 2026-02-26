using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAttend.Domain.ValueObjects
{
    /// <summary>
    /// Defines a valid work window (e.g., Monday 08:00 to 17:00).
    /// </summary>
    public record WorkSchedule(DayOfWeek Day, TimeOnly StartTime, TimeOnly EndTime);
}

