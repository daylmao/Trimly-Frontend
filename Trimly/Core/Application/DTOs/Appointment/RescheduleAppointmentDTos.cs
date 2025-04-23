namespace Trimly.Core.Application.DTOs.Appointment;

public class RescheduleAppointmentDTos
{
    private DateTime? _newStartDateTime;
    private DateTime? _newEndDateTime;

    public DateTime? NewStartDateTime
    {
        get => _newStartDateTime;
        set => _newStartDateTime = value?.ToUniversalTime();
    }
    
    public DateTime? NewEndDateTime
    {
        get => _newEndDateTime;
        set => _newEndDateTime = value?.ToUniversalTime();
    }
}