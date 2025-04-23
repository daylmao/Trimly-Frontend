using Trimly.Core.Application.DTOs;
using Trimly.Core.Application.DTOs.Appointment;
using Trimly.Core.Application.DTOs.Reviews;
using Trimly.Core.Application.Pagination;

namespace Trimly.Infrastructure.Api;

public interface IReviewsService
{

    Task<ApiResponse<ReviewsDTos>> CreateAsync(CreateReviewsDTO request);
    
    Task<ApiResponse<PagedResponse<ReviewsDTos>>> PaginationReviews(int pageNumber, int pageSize );
    
}