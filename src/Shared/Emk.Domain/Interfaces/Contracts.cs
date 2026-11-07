using Emk.Domain.DTOs;

namespace Emk.Domain.Interfaces
{
    /// <summary>Сетевой клиент шлюза (реализация — Emk.Infrastructure).</summary>
    public interface IEmkApiClient
    {
        Task<SlotsResponse> GetDoctorSlotsAsync(DateOnly date, Specialty? specialty = null, CancellationToken ct = default);
        Task<Appointment> BookAppointmentAsync(AppointmentRequest request, CancellationToken ct = default);
    }

    /// <summary>Локальный кэш расписания для офлайн-режима (реализация — Emk.Infrastructure).</summary>
    public interface ISlotCache
    {
        Task SaveAsync(SlotsResponse slots, CancellationToken ct = default);
        Task<SlotsResponse?> LoadAsync(DateOnly date, CancellationToken ct = default);
    }

    /// <summary>Источник JWT-токена для запросов к шлюзу (реализация — Emk.Security).</summary>
    public interface ITokenProvider
    {
        string? GetAccessToken();
    }

    /// <summary>Бизнес-логика записи к врачу (реализация — Emk.Core).</summary>
    public interface IAppointmentService
    {
        Task<(SlotsResponse Slots, bool FromCache)> GetSlotsAsync(DateOnly date, CancellationToken ct = default);
        Task<Appointment> BookAsync(Slot slot, string patientId, string? complaint, CancellationToken ct = default);
    }

    /// <summary>Хранилище расписания на стороне шлюза (реализация — Emk.Gateway.Data).</summary>
    public interface IScheduleRepository
    {
        IReadOnlyList<Slot> GetFreeSlots(DateOnly date, Specialty? specialty);
        Appointment? TryBook(AppointmentRequest request);
    }
}
