using System.Text.Json.Serialization;

namespace Emk.Domain.DTOs
{
    /// <summary>Специальность врача (components/schemas/Specialty из openapi.yaml).</summary>
    [JsonConverter(typeof(JsonStringEnumConverter<Specialty>))]
    public enum Specialty
    {
        THERAPIST,
        DENTIST,
        OPHTHALMOLOGIST,
        SURGEON,
        PSYCHOLOGIST
    }

    public record Doctor(Guid DoctorId, string FullName, Specialty Specialty, string? Room);

    public record Slot(Guid SlotId, Doctor Doctor, DateTimeOffset StartTime, DateTimeOffset EndTime);

    public record SlotsResponse(DateOnly Date, IReadOnlyList<Slot> Slots);
}
