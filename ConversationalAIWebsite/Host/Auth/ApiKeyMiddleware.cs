using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ConversationalAIWebsite.Host.Auth
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ApiKeyHeaderName = "X-API-Key";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IApiKeyValidator validator)
        {
            // Only act if the API key header is present.
            if (context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                var hashedKey = validator.HashApiKey(extractedApiKey);
                var user = await validator.ValidateApiKeyAsync(hashedKey);
                if (user == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid or expired API key.");
                    return;
                }

                // Create claims based on your validated user.
                var claims = new List<Claim>
                {
                    // Assumes that your 'user' object has properties Id and Username.
                    new Claim(ClaimTypes.Name, user.Username),
                    // Add any additional claims as needed.
                };

                // Create a ClaimsIdentity and assign it to HttpContext.User.
                var identity = new ClaimsIdentity(claims, "ApiKey");
                context.User = new ClaimsPrincipal(identity);
            }

            // Continue processing.
            await _next(context);
        }
    }
}
