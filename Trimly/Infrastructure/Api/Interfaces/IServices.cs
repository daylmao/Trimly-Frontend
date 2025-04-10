using Trimly.Core.Application.DTOs;
using Trimly.Core.Application.DTOs.Schedules;
using Trimly.Core.Application.DTOs.Services;

namespace Trimly.Infrastructure.Api;

public interface IServices
{
    Task<ApiResponse<ServicesDTos>> CreateService(CreateServiceDto request);
    Task<ApiResponse<List<ServicesDTos>>> GetServicesByCompanyId(Guid companyId);
}