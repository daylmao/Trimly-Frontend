namespace Trimly.Core.Application.DTOs.Appointment;

public class CreateAppointmentsDTos
{
    public DateTime StarDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public Guid? ServiceId { get; set; }
}