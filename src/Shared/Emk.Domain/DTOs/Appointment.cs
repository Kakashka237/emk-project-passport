using System.Text.Json.Serialization;

namespace Emk.Domain.DTOs
{
    public record AppointmentRequest(Guid SlotId, string PatientId, string? Complaint = null);

    [JsonConverter(typeof(JsonStringEnumConverter<AppointmentStatus>))]
    public enum AppointmentStatus
    {
        BOOKED,
        CANCELLED,
        COMPLETED
    }

    public record Appointment(Guid AppointmentId, Slot Slot, string PatientId, AppointmentStatus Status, DateTimeOffset CreatedAt);

    public record ApiError(string Code, string Message);
}
