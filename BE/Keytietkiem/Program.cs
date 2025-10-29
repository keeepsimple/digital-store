using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Keytietkiem.Infrastructure;
using Keytietkiem.Models;
using Keytietkiem.Options;
using Keytietkiem.Repositories;
using Keytietkiem.Services;
using Keytietkiem.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ===== Connection string =====
var connStr = builder.Configuration.GetConnectionString("MyCnn");

// ===== DI =====
builder.Services.AddDbContext<KeytietkiemDbContext>(opt =>
    opt.UseSqlServer(connStr));

// Clock (mockable for tests)
builder.Services.AddSingleton<IClock, SystemClock>();

// ===== Controllers + JSON camelCase + Enum string =====
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // Nếu bạn dùng DateOnly/TimeOnly, thêm converter custom tại đây
        // o.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

// ===== Uniform ModelState error => { message: "..." } =====
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var first = context.ModelState
            .Where(kv => kv.Value?.Errors.Count > 0)
            .Select(kv => kv.Value!.Errors[0].ErrorMessage)
            .FirstOrDefault() ?? "Dữ liệu không hợp lệ";
        return new BadRequestObjectResult(new { message = first });
    };
});

// ===== Configuration Options =====
builder.Services.Configure<MailConfig>(builder.Configuration.GetSection("MailConfig"));
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

// ===== Memory Cache =====
builder.Services.AddMemoryCache();

// ===== Repositories =====
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// ===== Services =====
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// ===== CORS (một policy duy nhất) =====
const string FrontendCors = "Frontend";
var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
                 ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(o => o.AddPolicy(FrontendCors, p =>
    p.WithOrigins(corsOrigins)
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials()
     .WithExposedHeaders("Content-Disposition") // phục vụ export CSV
));

// ===== JWT Authentication =====
var jwtSecretKey = builder.Configuration["JwtConfig:SecretKey"]
    ?? throw new InvalidOperationException("JwtConfig:SecretKey not found in appsettings.json");
var jwtIssuer = builder.Configuration["JwtConfig:Issuer"]
    ?? throw new InvalidOperationException("JwtConfig:Issuer not found in appsettings.json");
var jwtAudience = builder.Configuration["JwtConfig:Audience"]
    ?? throw new InvalidOperationException("JwtConfig:Audience not found in appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ===== Swagger =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===== Global exception -> { message: "..." } =====
app.UseExceptionHandler(exApp =>
{
    exApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json; charset=utf-8";
        var payload = JsonSerializer.Serialize(new { message = "Đã có lỗi hệ thống. Vui lòng thử lại sau." });
        await context.Response.WriteAsync(payload);
    });
});

// ===== Dev tools =====
app.UseSwagger();
app.UseSwaggerUI();

// TẮT trong dev nếu chỉ dùng HTTP để tránh redirect gây CORS
// app.UseHttpsRedirection();

app.UseCors(FrontendCors);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
