using JobNexus.Data;
using JobNexus.Extensions;
using JobNexus.Helpers.Interceptors;
using JobNexus.Seed;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
        ReferenceHandler.IgnoreCycles;
}); ;

// Connect to DB
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .AddInterceptors(new TimestampInterceptor()));

// Configure Identity   
builder.Services.AddIdentityAuth();

// Enforce Global Authentication
builder.Services.AddGlobalAuth();

// Configure JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Dependency Injection for Repositories
builder.Services.AddRepositories();

// Dependency Injection for Services
builder.Services.AddServices();

// Configure Swagger with Bearer Authentication
builder.Services.AddSwaggerBearerAuth();

var app = builder.Build();

// Seed Roles and Admin User
await app.Services.Initialize();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Open API v1");
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
