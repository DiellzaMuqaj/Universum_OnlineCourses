using Microsoft.EntityFrameworkCore;
using Universum_OnlineCourses.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Universum_OnlineCourses.Aplication.Services;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Add scoped services
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<TokenService>();

// Configure DbContext with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Settings
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

// Stripe configuration
Stripe.StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Listen to Railway PORT
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    app.Urls.Add($"http://0.0.0.0:{port}");
}

// ✅ Swagger available in all environments (production-safe version)
var enableSwagger = Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true";

if (app.Environment.IsDevelopment() || enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Universum API V1");
        c.RoutePrefix = "swagger"; // Swagger URL: /swagger
    });
}

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
