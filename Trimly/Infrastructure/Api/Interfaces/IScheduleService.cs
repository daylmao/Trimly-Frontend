using Trimly.Core.Application.DTOs;
using Trimly.Core.Application.DTOs.Schedules;

namespace Trimly.Infrastructure.Api;

public interface IScheduleService
{
    Task<ApiResponse<ScheduleDTO>> CreateSchedule(CreateScheduleDTO request);
    Task<ApiResponse<List<ScheduleDTO>>> GetSchedulesByCompany(Guid companyId);
    Task<DeleteResult> DeleteScheduleById(Guid scheduleId);
    Task<ApiResponse<ScheduleDTO>> UpdateScheduleById(Guid scheduleId, UpdateSchedulesDTos request);
}