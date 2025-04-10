namespace Trimly.Core.Application.DTOs.Schedules;

public class CreateScheduleDTO
{
    public Weekday days { get; set; }
    public TimeOnly OpeningTime { get; set; }
    public TimeOnly ClosingTime { get; set; }
    public string? Notes { get; set; }
    public Guid? RegisteredCompanyId { get; set; }
}