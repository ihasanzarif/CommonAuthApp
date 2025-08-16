using CommonAuthApp.Web.Components;
using CommonAuthApp.Web.Middleware;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// HttpClient for Auth Service validation
builder.Services.AddHttpClient();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
// YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddControllers();
// Swagger
builder.Services.AddEndpointsApiExplorer();
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

app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<RoleAuthorizationMiddleware>();

app.MapReverseProxy();
app.MapRazorPages();

app.Run();
