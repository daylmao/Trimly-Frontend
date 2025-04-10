namespace Trimly.Core.Application.DTOs.Schedules;

public class DeleteResult
{
    public Guid? DeletedId { get; set; }
    public bool Success => DeletedId != null && DeletedId != Guid.Empty;
    public string ErrorMessage { get; set; }
}