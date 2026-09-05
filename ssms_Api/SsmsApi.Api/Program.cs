using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SsmsApi.Domain.Entities;
using SsmsApi.Infrastructure.Persistence;
using SsmsApi.Application.Interfaces;
using SsmsApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);
// Controllers
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200"
        )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // needed since your auth uses cookies
    });
});
builder.Services.AddScoped<ITokenService, TokenService>();
// Swagger / OpenAPI
builder.Services.AddOpenApi();
builder.Services.AddScoped<IAuthService, AuthService>();
// Database — connects SsmsDbContext to Postgres via the connection string
builder.Services.AddDbContext<SsmsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IEmailService, EmailService>();
// Identity core — registers UserManager<ApplicationUser>, RoleManager<IdentityRole<Guid>>,
// and wires them to SsmsDbContext as the storage backend.
builder.Services.AddDataProtection();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IMaterialItemService, MaterialItemService>();
builder.Services.AddScoped<IMaterialRequestService, MaterialRequestService>();
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPaymentGatewayService, FakeChapaPaymentGatewayService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<SsmsDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("access_token"))
            {
                context.Token = context.Request.Cookies["access_token"];
            }
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    await RoleSeeder.SeedRolesAsync(scope.ServiceProvider);
}
// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // serves UI at /scalar
}
app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");   // <-- ADD THIS LINE
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();