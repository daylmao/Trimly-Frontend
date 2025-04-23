namespace Trimly.Core.Application.DTOs.Reviews;

public class ReviewsResponseDTos
{
    public IEnumerable<ReviewsDTos> Items { get; set; } = new List<ReviewsDTos>();
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
}