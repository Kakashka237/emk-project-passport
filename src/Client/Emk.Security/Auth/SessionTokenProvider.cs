using Emk.Domain.Interfaces;

namespace Emk.Security.Auth
{
    /// <summary>
    /// Хранит JWT текущего сеанса в памяти и не отдает просроченный токен.
    /// Токен не записывается на диск, чтобы его нельзя было похитить из файлов профиля.
    /// </summary>
    public class SessionTokenProvider : ITokenProvider
    {
        private readonly TimeProvider _clock;
        private string? _token;
        private DateTimeOffset _expiresAt;

        public SessionTokenProvider(TimeProvider clock) => _clock = clock;

        public void SignIn(string accessToken, TimeSpan lifetime)
        {
            _token = accessToken;
            _expiresAt = _clock.GetUtcNow() + lifetime;
        }

        public void SignOut() => _token = null;

        public string? GetAccessToken() => _clock.GetUtcNow() < _expiresAt ? _token : null;
    }
}
