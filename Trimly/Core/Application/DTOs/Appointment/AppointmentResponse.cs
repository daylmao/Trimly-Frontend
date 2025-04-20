using Trimly.Core.Domain.Enums;

namespace Trimly.Core.Application.DTOs.Appointment;

public class AppointmentResponse
{
    public Guid AppointmentId { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public AppointmentStatus AppointmentStatus { get; set; }
    public Guid ServiceId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }


}