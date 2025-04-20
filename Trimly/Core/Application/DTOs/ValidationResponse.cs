using Trimly.Core.Application.DTOs.Schedules;

namespace Trimly.Core.Application;

public class ValidationResponse
{
    public bool IsValid { get; set; }
    public List<ValidationMessage> Errors { get; set; } = new();
}