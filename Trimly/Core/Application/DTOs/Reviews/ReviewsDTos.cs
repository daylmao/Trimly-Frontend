using System.Text.Json.Serialization;

namespace Trimly.Core.Application.DTOs.Reviews;

public class ReviewsDTos
{
    public Guid? ReviewId { get; set; }
    
    public string? Title { get; set; }
    
    public string? Comment { get; set; }
    
    public int Rating { get; set; }
    
    public Guid? RegisteredCompanyId { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdateAt { get; set; }
}