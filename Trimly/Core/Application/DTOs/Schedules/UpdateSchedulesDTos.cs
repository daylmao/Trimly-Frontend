using Trimly.Core.Application.Utils;

namespace Trimly.Core.Application.DTOs.Schedules;

public class UpdateSchedulesDTos
{
    public Weekday? days { get; set; }
    public TimeOnly OpeningTime { get; set; }
    public TimeOnly ClosingTime { get; set; }
    public string? Notes { get; set; }
    public Status? IsHoliday { get; set; } = null;

}