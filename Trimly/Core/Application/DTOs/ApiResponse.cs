using Trimly.Core.Application.DTOs.Schedules;

namespace Trimly.Core.Application.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public string? ErrorMessage { get; set; }

    public List<ValidationMessage>? ValidationMessages { get; set; }
}