using System.Text.Json;
using Emk.Domain.DTOs;
using Emk.Domain.Interfaces;
using Emk.Infrastructure.ApiClient;

namespace Emk.Infrastructure.LocalStorage
{
    /// <summary>Кэш расписания в JSON-файлах: %LOCALAPPDATA%\Emk\cache\slots-ГГГГ-ММ-ДД.json.</summary>
    public class JsonSlotCache : ISlotCache
    {
        private readonly string _folder;

        public JsonSlotCache(string? folder = null)
        {
            _folder = folder ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Emk", "cache");
            Directory.CreateDirectory(_folder);
        }

        public async Task SaveAsync(SlotsResponse slots, CancellationToken ct = default)
        {
            await using var file = File.Create(PathFor(slots.Date));
            await JsonSerializer.SerializeAsync(file, slots, EmkApiClient.JsonOptions, ct);
        }

        public async Task<SlotsResponse?> LoadAsync(DateOnly date, CancellationToken ct = default)
        {
            var path = PathFor(date);
            if (!File.Exists(path))
                return null;
            await using var file = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<SlotsResponse>(file, EmkApiClient.JsonOptions, ct);
        }

        private string PathFor(DateOnly date) => Path.Combine(_folder, $"slots-{date:yyyy-MM-dd}.json");
    }
}
