using Trimly.Core.Application.DTOs.Services;
using Trimly.Core.Domain.Enums;

namespace Trimly.Core.Application.DTOs.Appointment;

public class AppointmentDTos
{
    public Guid AppointmentId { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public AppointmentStatus AppointmentStatus { get; set; }
    public Guid ServiceId { get; set; }
    
    public ServicesDTos? Services { get; set; }
}