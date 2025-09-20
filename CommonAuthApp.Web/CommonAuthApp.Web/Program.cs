using CommonAuthApp.Web.Components;
using CommonAuthApp.Web.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Gateway",
        Version = "v1",
        Description = "Gateway for Admin & Staff services"
    });
});

string secret = builder.Configuration.GetValue<string>("Jwt:Secret");
string issuer = builder.Configuration.GetValue<string>("Jwt:Issuer");
string audience = builder.Configuration.GetValue<string>("Jwt:Audience");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };
});
builder.Services.AddAuthorization();
//  Add HttpClient for RoleAuthorizationMiddleware
builder.Services.AddHttpClient("GatewayClient")
    .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // Reuse handlers
    .AddTransientHttpErrorPolicy(p => p.RetryAsync(3)) // Polly retry
    .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");
    });
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RoleAuthorizationMiddleware>();

app.Run();
