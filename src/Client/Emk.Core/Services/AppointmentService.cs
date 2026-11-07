using Emk.Core.Validators;
using Emk.Domain.DTOs;
using Emk.Domain.Interfaces;

namespace Emk.Core.Services
{
    /// <summary>
    /// Доменные правила записи к врачу. Зависит только от интерфейсов Emk.Domain,
    /// поэтому не знает ни про HTTP, ни про файлы, ни про WPF.
    /// </summary>
    public class AppointmentService : IAppointmentService
    {
        private readonly IEmkApiClient _api;
        private readonly ISlotCache _cache;
        private readonly TimeProvider _clock;

        public AppointmentService(IEmkApiClient api, ISlotCache cache, TimeProvider clock)
        {
            _api = api;
            _cache = cache;
            _clock = clock;
        }

        public async Task<(SlotsResponse Slots, bool FromCache)> GetSlotsAsync(DateOnly date, CancellationToken ct = default)
        {
            try
            {
                var slots = await _api.GetDoctorSlotsAsync(date, ct: ct);
                await _cache.SaveAsync(slots, ct);
                return (slots, false);
            }
            catch (HttpRequestException)
            {
                // шлюз недоступен — показываем последнее сохраненное расписание
                var cached = await _cache.LoadAsync(date, ct);
                if (cached is null) throw;
                return (cached, true);
            }
        }

        public Task<Appointment> BookAsync(Slot slot, string patientId, string? complaint, CancellationToken ct = default)
        {
            var request = new AppointmentRequest(slot.SlotId, patientId, complaint);
            var errors = AppointmentValidator.Validate(request).ToList();
            if (slot.StartTime <= _clock.GetUtcNow())
                errors.Add("Нельзя записаться на прошедшее время");
            if (errors.Count > 0)
                throw new ArgumentException(string.Join("; ", errors));

            return _api.BookAppointmentAsync(request, ct);
        }
    }
}
