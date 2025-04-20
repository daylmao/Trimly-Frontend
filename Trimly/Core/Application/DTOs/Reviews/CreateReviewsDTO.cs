namespace Trimly.Core.Application.DTOs.Reviews;

public class CreateReviewsDTO
{
   public string? Title { get; set; }
   public string? Comment { get; set; }
   public int Rating { get; set; }
   public Guid? RegisteredCompanyId { get; set; }
}
