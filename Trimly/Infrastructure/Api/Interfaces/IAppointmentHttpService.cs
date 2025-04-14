using Trimly.Core.Application.DTOs;
using Trimly.Core.Application.DTOs.Appointment;
using Trimly.Core.Application.Pagination;
using Trimly.Core.Domain.Enums;

namespace Trimly.Infrastructure.Api;

public interface IAppointmentHttpService
{
    Task<ApiResponse<bool>> CanceledAppointmentAsync(Guid appointmentId);

    Task<ApiResponse<string>> ConfirmAppointmentAsync(Guid appointmentId, Guid serviceId);

    Task<ApiResponse<PagedResponse<AppointmentDTos>>> PaginationAppointmentsAsync(
        int pageNumber, 
        int pageSize );

    Task<ApiResponse<IEnumerable<AppointmentDTos>>> GetAppointmentsByStatusAsync(AppointmentStatus status);
}