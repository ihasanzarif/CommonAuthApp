using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace CommonAuthApp.Web.Controllers
{
    public class GatewayController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly HttpClient _client;

        public GatewayController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _client = _httpClientFactory.CreateClient();
        }

        // --- LOGIN ROUTES ---
        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin([FromBody] object loginRequest)
        {
            return await ForwardRequestAsync("authCluster", "api/auth/authenticate", loginRequest);
        }

        [HttpPost("staff/login")]
        public async Task<IActionResult> StaffLogin([FromBody] object loginRequest)
        {
            return await ForwardRequestAsync("staffAuthCluster", "api/staffauth/authenticate", loginRequest);
        }

        // --- SECURED DATA ROUTES ---
        [HttpGet("admin/data/{**path}")]
        public async Task<IActionResult> AdminData(string path)
        {
            if (!await ValidateTokenAsync("Admin")) return Unauthorized(new { Message = "Invalid admin token" });

            return await ForwardGetAsync("adminCluster", $"api/admin/{path}");
        }

        [HttpGet("staff/data/{**path}")]
        public async Task<IActionResult> StaffData(string path)
        {
            if (!await ValidateTokenAsync("Staff")) return Unauthorized(new { Message = "Invalid staff token" });

            return await ForwardGetAsync("staffCluster", $"api/staff/{path}");
        }

        // --- TOKEN VALIDATION ---
        private async Task<bool> ValidateTokenAsync(string userType)
        {
            var token = Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                return false;

            var validateUrl = userType == "Admin"
                ? _config["AuthenticationService:AdminValidateUrl"]
                : _config["AuthenticationService:StaffValidateUrl"];

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(validateUrl);
            return response.IsSuccessStatusCode;
        }

        // --- FORWARD HELPERS ---
        private async Task<IActionResult> ForwardRequestAsync(string clusterKey, string path, object content)
        {
            var url = GetClusterUrl(clusterKey) + path;
            var response = await _client.PostAsJsonAsync(url, content);
            var data = await response.Content.ReadAsStringAsync();
            return Content(data, response.Content.Headers.ContentType?.ToString() ?? "application/json");
        }

        private async Task<IActionResult> ForwardGetAsync(string clusterKey, string path)
        {
            var url = GetClusterUrl(clusterKey) + path;
            var response = await _client.GetAsync(url);
            var data = await response.Content.ReadAsStringAsync();
            return Content(data, response.Content.Headers.ContentType?.ToString() ?? "application/json");
        }

        private string GetClusterUrl(string clusterKey)
        {
            var section = _config.GetSection($"ReverseProxy:Clusters:{clusterKey}:Destinations");
            var firstDest = section.GetChildren().First();
            return firstDest.GetValue<string>("Address");
        }
    }
}
