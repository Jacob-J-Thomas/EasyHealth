using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using AiPocWebsiteTemplateWithBackend.Host.Config;

namespace AiPocWebsiteTemplateWithBackend.Host.Auth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly string _appUsername;
        private readonly string _appPassword;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, AuthSettings settings) : base(options, logger, encoder)
        {
            _appUsername = settings.BasicAuthUsername;
            _appPassword = settings.BasicAuthPassword;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
            }

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (authHeader.Scheme != "Basic")
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Scheme"));
            }

            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':', 2);
            var username = credentials[0];
            var password = credentials[1];

            // Validate the username and password
            if (username != _appUsername || password != _appPassword) // Replace with your logic
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Credentials"));
            }

            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, "Basic");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "BasicAuthentication");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

}
