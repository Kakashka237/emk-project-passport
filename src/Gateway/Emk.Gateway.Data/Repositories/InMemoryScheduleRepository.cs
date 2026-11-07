using System.Collections.Concurrent;
using Emk.Domain.DTOs;
using Emk.Domain.Interfaces;

namespace Emk.Gateway.Data.Repositories
{
    /// <summary>Учебное хранилище расписания в памяти. В рабочей версии заменяется на PostgreSQL + EF Core.</summary>
    public class InMemoryScheduleRepository : IScheduleRepository
    {
        private static readonly Doctor[] Doctors =
        [
            new(Guid.Parse("0d9e8f7a-6b5c-4d3e-8f1a-2b3c4d5e6f70"), "Иванова Анна Петровна", Specialty.THERAPIST, "214"),
            new(Guid.Parse("1a2b3c4d-5e6f-4a1b-8c2d-3e4f5a6b7c80"), "Смирнов Олег Викторович", Specialty.DENTIST, "105")
        ];

        private readonly ConcurrentDictionary<Guid, Appointment> _booked = new();

        public IReadOnlyList<Slot> GetFreeSlots(DateOnly date, Specialty? specialty) =>
            (from doctor in Doctors
             where specialty is null || doctor.Specialty == specialty
             from hour in Enumerable.Range(9, 6)
             let start = new DateTimeOffset(date.ToDateTime(new TimeOnly(hour, 0)), TimeSpan.FromHours(3))
             let slot = new Slot(SlotId(doctor, start), doctor, start, start.AddMinutes(20))
             where !_booked.ContainsKey(slot.SlotId)
             select slot).ToList();

        public Appointment? TryBook(AppointmentRequest request)
        {
            var slot = Doctors
                .SelectMany(d => GetFreeSlots(DateOnly.FromDateTime(DateTime.Today), d.Specialty))
                .FirstOrDefault(s => s.SlotId == request.SlotId);
            if (slot is null)
                return null;

            var appointment = new Appointment(Guid.NewGuid(), slot, request.PatientId, AppointmentStatus.BOOKED, DateTimeOffset.UtcNow);
            return _booked.TryAdd(slot.SlotId, appointment) ? appointment : null;
        }

        // детерминированный идентификатор слота: врач + время начала
        private static Guid SlotId(Doctor doctor, DateTimeOffset start)
        {
            var bytes = doctor.DoctorId.ToByteArray();
            BitConverter.GetBytes(start.ToUnixTimeSeconds()).CopyTo(bytes, 8);
            return new Guid(bytes);
        }
    }
}
