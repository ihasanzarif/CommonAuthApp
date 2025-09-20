using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace CommonAuthApp.API.Handlers
{
    public class ClaimsFromHeaderAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public ClaimsFromHeaderAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-User-Claims", out var claimsHeader))
            {
                // Header not found, authentication fails.
                return Task.FromResult(AuthenticateResult.Fail("X-User-Claims header not found."));
            }

            try
            {
                var claimsDto = JsonSerializer.Deserialize<IEnumerable<ClaimDto>>(claimsHeader.ToString());
                var claims = claimsDto.Select(c => new Claim(c.Type, c.Value));
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                return Task.FromResult(AuthenticateResult.Fail(ex));
            }
        }

        private class ClaimDto
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }
}
