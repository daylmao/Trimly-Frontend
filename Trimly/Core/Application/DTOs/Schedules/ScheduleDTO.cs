using System.Text.Json.Serialization;
using Trimly.Core.Application.Utils;

namespace Trimly.Core.Application.DTOs.Schedules;

public class ScheduleDTO
{
    public Guid schedulesId { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Weekday? days { get; set; }

    public TimeOnly OpeningTime { get; set; } 
    public TimeOnly ClosingTime { get; set; }
    public Status? IsHoliday{ get; set; }
    public string? Notes { get; set; }
    public string? RegisteredCompanyId { get; set; }
}