using System.Text.Json;

namespace CommonAuthApp.Web.Middleware
{
    public class RoleAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly HashSet<string> _publicEndpoints;

        public RoleAuthorizationMiddleware(RequestDelegate next, IHttpClientFactory clientFactory, IConfiguration config)
        {
            _next = next;
            _httpClient = clientFactory.CreateClient("GatewayClient"); // Named client with Polly policies
            _config = config;

            // Load public endpoints from config into a set for fast lookup
            _publicEndpoints = _config
                .GetSection("Gateway:PublicEndpoints")
                .Get<string[]>()
                ?.Select(p => p.ToLowerInvariant())
                .ToHashSet() ?? new HashSet<string>();
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLowerInvariant();

            // 🔹 Allow requests that match public endpoints
            if (_publicEndpoints.Any(pub => path.StartsWith(pub)))
            {
                await ForwardRequest(context);
                return;
            }

            // 🔹 Require JWT for everything else
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: A valid token is required.");
                return;
            }

            // 🔹 Forward authenticated requests
            await ForwardRequest(context);
        }

        private async Task ForwardRequest(HttpContext context)
        {
            var serviceRoutes = _config.GetSection("ServiceRoutes").GetChildren();

            foreach (var route in serviceRoutes)
            {
                var pathPrefix = route["PathPrefix"];
                var destinations = route.GetSection("Destinations").Get<string[]>();

                if (string.IsNullOrWhiteSpace(pathPrefix) || destinations == null || destinations.Length == 0)
                    continue;

                if (context.Request.Path.StartsWithSegments(pathPrefix, out var remainingPath))
                {
                    foreach (var destination in destinations)
                    {
                        try
                        {
                            var targetUri = $"{destination.TrimEnd('/')}{pathPrefix}{remainingPath}{context.Request.QueryString}";

                            using var requestMessage = BuildRequestMessage(context, targetUri);

                            // 🔹 Add claims header
                            if (context.User.Identity?.IsAuthenticated ?? false)
                            {
                                var claims = context.User.Claims.Select(c => new { c.Type, c.Value });
                                var claimsJson = JsonSerializer.Serialize(claims);
                                requestMessage.Headers.TryAddWithoutValidation("X-User-Claims", claimsJson);
                            }

                            using var responseMessage = await _httpClient.SendAsync(
                                requestMessage,
                                HttpCompletionOption.ResponseHeadersRead,
                                context.RequestAborted
                            );

                            // 🔹 Stream response body
                            await CopyResponseAsync(context, responseMessage);
                            return;
                        }
                        catch (HttpRequestException)
                        {
                            continue; // Try next destination
                        }
                    }

                    // All destinations failed
                    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    await context.Response.WriteAsync("All destinations are unavailable.");
                    return;
                }
            }

            // No matching route → continue normal pipeline
            await _next(context);
        }

        private HttpRequestMessage BuildRequestMessage(HttpContext context, string targetUri)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = new HttpMethod(context.Request.Method),
                RequestUri = new Uri(targetUri)
            };

            // Copy headers except Authorization
            foreach (var header in context.Request.Headers.Where(h => !h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)))
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }

            // Copy body if present
            if (context.Request.ContentLength > 0 || context.Request.Body.CanRead)
            {
                requestMessage.Content = new StreamContent(context.Request.Body);

                foreach (var header in context.Request.Headers)
                {
                    if (header.Key.StartsWith("Content-", StringComparison.OrdinalIgnoreCase))
                    {
                        requestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                    }
                }
            }

            return requestMessage;
        }

        private static async Task CopyResponseAsync(HttpContext context, HttpResponseMessage responseMessage)
        {
            context.Response.StatusCode = (int)responseMessage.StatusCode;

            foreach (var header in responseMessage.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            // Remove conflicting headers
            context.Response.Headers.Remove("transfer-encoding");

            // Ensure the content type is set
            if (!context.Response.Headers.ContainsKey("Content-Type"))
                context.Response.ContentType = responseMessage.Content.Headers.ContentType?.ToString() ?? "application/json";

            // Copy the content as string to ensure proper JSON response
            var contentString = await responseMessage.Content.ReadAsStringAsync();
            await context.Response.WriteAsync(contentString);
        }
    }
}
