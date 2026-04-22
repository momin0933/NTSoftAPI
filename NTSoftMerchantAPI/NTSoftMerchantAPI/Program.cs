using Microsoft.IdentityModel.Tokens;
using NTSoftMerchantAPI.BusinessLayer.Interface;
using NTSoftMerchantAPI.BusinessLayer.Manager;
using NTSoftMerchantAPI.BusinessLayer.Service;
using NTSoftMerchantAPI.BusinessLayer.TenantService;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Services
builder.Services.AddScoped<NTSoftDbContextFactory>();
builder.Services.AddScoped<NTSoftDbContext>(provider =>
{
    var factory = provider.GetRequiredService<NTSoftDbContextFactory>();
    return factory.CreateDbContext();
});

// Dependency Injection
builder.Services.AddTransient<IDapperService, DapperService>();  // ✅ Interface registration
builder.Services.AddScoped<ICommonService, CommonService>();      // ✅ Add this
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IUser, UserManager>();
builder.Services.AddScoped<ITenantStore, TenantStore>();

// Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT:Key not configured");
var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT:Issuer not configured");
var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT:Audience not configured");

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = audience,
            ValidIssuer = issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero
        };
    });

// CORS - Load from configuration
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
    ?? new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseMiddleware<TenantMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
