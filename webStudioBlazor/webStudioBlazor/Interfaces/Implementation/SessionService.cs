namespace webStudioBlazor.Interfaces.Implementation
{
    using Microsoft.AspNetCore.Http;

    using webStudioBlazor.Interfaces.Contract;

    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CookieName = "CartSessionKey";

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetSessionKey()
        {
            var context = _httpContextAccessor.HttpContext!;
            var cookies = context.Request.Cookies;

            if (cookies.TryGetValue(CookieName, out var key) && !string.IsNullOrEmpty(key))
                return key;

            // Якщо ключу немає — створюємо новий
            key = Guid.NewGuid().ToString("N");

            // Зберігаємо cookie на 30 днів
            context.Response.Cookies.Append(
                CookieName,
                key,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = context.Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                });

            return key;
        }
    }

}
