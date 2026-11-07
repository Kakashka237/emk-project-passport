using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Emk.Domain.DTOs;
using Emk.Domain.Interfaces;

namespace Emk.Infrastructure.ApiClient
{
    /// <summary>Клиент интеграционного шлюза по контракту openapi.yaml (ПР6).</summary>
    public class EmkApiClient : IEmkApiClient
    {
        public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly HttpClient _http;
        private readonly ITokenProvider _tokens;

        public EmkApiClient(HttpClient http, ITokenProvider tokens)
        {
            _http = http;
            _tokens = tokens;
        }

        public async Task<SlotsResponse> GetDoctorSlotsAsync(DateOnly date, Specialty? specialty = null, CancellationToken ct = default)
        {
            var url = $"doctors/slots?date={date:yyyy-MM-dd}" + (specialty is null ? "" : $"&specialty={specialty}");
            using var response = await SendAsync(new HttpRequestMessage(HttpMethod.Get, url), ct);
            return (await response.Content.ReadFromJsonAsync<SlotsResponse>(JsonOptions, ct))!;
        }

        public async Task<Appointment> BookAppointmentAsync(AppointmentRequest request, CancellationToken ct = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "appointments/book") { Content = JsonContent.Create(request, options: JsonOptions) };
            using var response = await SendAsync(message, ct);
            return (await response.Content.ReadFromJsonAsync<Appointment>(JsonOptions, ct))!;
        }

        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken ct)
        {
            var token = _tokens.GetAccessToken();
            if (token is not null)
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(message, ct);
            if (response.IsSuccessStatusCode)
                return response;

            var error = await response.Content.ReadFromJsonAsync<ApiError>(JsonOptions, ct);
            throw new EmkApiException(response.StatusCode, error?.Code ?? "UNKNOWN", error?.Message ?? "");
        }
    }

    public class EmkApiException(HttpStatusCode status, string code, string message) : Exception(message)
    {
        public HttpStatusCode StatusCode { get; } = status;
        public string Code { get; } = code;
    }
}
