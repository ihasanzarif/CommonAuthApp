using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Yarp.ReverseProxy.Configuration;

namespace CommonAuthApp.Web.Middleware
{
    public class RoleAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;

        public RoleAuthorizationMiddleware(RequestDelegate next, IHttpClientFactory clientFactory, IConfiguration config)
        {
            _next = next;
            _clientFactory = clientFactory;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            var routeName = context.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;

            if (string.IsNullOrEmpty(routeName))
            {
                await _next(context);
                return;
            }

            var allowedRolesCsv = _config[$"ReverseProxy:Routes:{routeName}:Metadata:AllowedRoles"];
            if (string.IsNullOrWhiteSpace(allowedRolesCsv))
            {
                await _next(context); // public route
                return;
            }

            // Expect Bearer token
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Missing Authorization header.");
                return;
            }

            var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authHeader.Substring("Bearer ".Length)
                : authHeader;

            // Pick correct Auth service
            var validateUrl = PickValidateUrlFromAllowedRoles(allowedRolesCsv);
            if (string.IsNullOrWhiteSpace(validateUrl))
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Gateway misconfiguration: missing validation URL.");
                return;
            }

            // Call Auth service
            var client = _clientFactory.CreateClient();
            var req = new HttpRequestMessage(HttpMethod.Get, validateUrl);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage authResp;
            try
            {
                authResp = await client.SendAsync(req);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status502BadGateway;
                await context.Response.WriteAsync($"Auth service unreachable: {ex.Message}");
                return;
            }

            if (!authResp.IsSuccessStatusCode)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token.");
                return;
            }

            // Check role in JWT
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwt = null;
            try { jwt = handler.ReadJwtToken(token); } catch { }

            var userRoles = GetRoles(jwt);
            var allowed = allowedRolesCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (!userRoles.Any(r => allowed.Contains(r, StringComparer.OrdinalIgnoreCase)))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: missing required role.");
                return;
            }

            await _next(context);
        }

        private string? PickValidateUrlFromAllowedRoles(string allowedRolesCsv)
        {
            var roles = allowedRolesCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // If any Admin is required, use Admin auth service; otherwise use Staff auth service
            if (roles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
                return _config["AuthenticationService:AdminValidateUrl"];

            return _config["AuthenticationService:StaffValidateUrl"];
        }

        private static IEnumerable<string> GetRoles(JwtSecurityToken? jwt)
        {
            if (jwt is null) return Enumerable.Empty<string>();

            var roleClaimTypes = new[]
            {
                "role",
                "roles",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            };

            return jwt.Claims
                .Where(c => roleClaimTypes.Contains(c.Type, StringComparer.OrdinalIgnoreCase))
                .SelectMany(c => c.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }
    }
}
