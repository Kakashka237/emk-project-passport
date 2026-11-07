using Emk.Core.Services;
using Emk.Core.Validators;
using Emk.Domain.DTOs;
using Emk.Domain.Interfaces;

namespace Emk.Core.Tests
{
    public class AppointmentServiceTests
    {
        private static readonly DateOnly Day = new(2026, 10, 22);
        private static readonly Slot Morning = new(Guid.NewGuid(),
            new Doctor(Guid.NewGuid(), "Иванова Анна Петровна", Specialty.THERAPIST, "214"),
            new DateTimeOffset(2026, 10, 22, 9, 0, 0, TimeSpan.FromHours(3)),
            new DateTimeOffset(2026, 10, 22, 9, 20, 0, TimeSpan.FromHours(3)));

        [Theory]
        [InlineData("STU-2023-00417", true)]
        [InlineData("2023-417", false)]
        public void Validator_ChecksPatientIdPattern(string patientId, bool valid)
        {
            var errors = AppointmentValidator.Validate(new AppointmentRequest(Guid.NewGuid(), patientId));
            Assert.Equal(valid, errors.Count == 0);
        }

        [Fact]
        public async Task GetSlots_GatewayDown_ReturnsCachedSchedule()
        {
            var cache = new MemoryCache();
            await cache.SaveAsync(new SlotsResponse(Day, [Morning]));
            var service = new AppointmentService(new OfflineApi(), cache, new FixedClock(Day));

            var (slots, fromCache) = await service.GetSlotsAsync(Day);

            Assert.True(fromCache);
            Assert.Single(slots.Slots);
        }

        [Fact]
        public async Task Book_PastSlot_IsRejectedBeforeCallingGateway()
        {
            var service = new AppointmentService(new OfflineApi(), new MemoryCache(), new FixedClock(Day.AddDays(1)));

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.BookAsync(Morning, "STU-2023-00417", null));
            Assert.Contains("прошедшее время", ex.Message);
        }

        private sealed class OfflineApi : IEmkApiClient
        {
            public Task<SlotsResponse> GetDoctorSlotsAsync(DateOnly date, Specialty? specialty = null, CancellationToken ct = default) =>
                throw new HttpRequestException("Шлюз недоступен");
            public Task<Appointment> BookAppointmentAsync(AppointmentRequest request, CancellationToken ct = default) =>
                throw new HttpRequestException("Шлюз недоступен");
        }

        private sealed class MemoryCache : ISlotCache
        {
            private readonly Dictionary<DateOnly, SlotsResponse> _data = new();
            public Task SaveAsync(SlotsResponse slots, CancellationToken ct = default) { _data[slots.Date] = slots; return Task.CompletedTask; }
            public Task<SlotsResponse?> LoadAsync(DateOnly date, CancellationToken ct = default) => Task.FromResult(_data.GetValueOrDefault(date));
        }

        private sealed class FixedClock(DateOnly day) : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() => new(day.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        }
    }
}
