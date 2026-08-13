using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Manager;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.BusinessLayer.TenantService;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.Models.Apps.PropHUB;
using BMSAPI.BusinessLayer.Manager.AppManager.ProHUBManager;

var builder = WebApplication.CreateBuilder(args);

// Newtonsoft.Json enable
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

        // Keep property names as they are (PascalCase like 'Name')
        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
    });



// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddScoped<NTSoftDbContextFactory>();
//builder.Services.AddScoped<NTSoftDbContext>(provider =>
//{
//    var factory = provider.GetRequiredService<NTSoftDbContextFactory>();
//    return factory.CreateDbContext();
//});


//// Connection string add করুন
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//builder.Services.AddDbContext<NTSoftDbContext>(options =>
//    options.UseSqlServer(connectionString));

// dependency injection
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

// Factory
builder.Services.AddScoped<NTSoftDbContextFactory>();

// Dapper (IMPORTANT FIX)
builder.Services.AddScoped<IDapperService, DapperService>();

// Common service
builder.Services.AddScoped<ICommonService, CommonService>();

// Business services

builder.Services.AddScoped<ITenantStore, TenantStore>();

// Custom service
builder.Services.AddScoped<CustomService>();
// Tenant Provider
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

// Interface Scoped

builder.Services.AddScoped<IBkashManager, BkashManager>();
builder.Services.AddScoped<IUserManager, UserManager>();
//APP INTERFACES SCOPED
//PropHUB Applications
builder.Services.AddScoped<IUserRegistration,UserRegistrationManager>();


builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor(); // ⚠️ important


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, // 🔥 ADD THIS
        ValidateIssuerSigningKey = true,

        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

        ClockSkew = TimeSpan.Zero // 🔥 ADD THIS (important)
    };
});

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});




var app = builder.Build();

//app.Use(async (context, next) =>
//{
//    var sw = Stopwatch.StartNew();

//    await next();

//    sw.Stop();
//    Console.WriteLine($"TOTAL PIPELINE: {sw.ElapsedMilliseconds} ms");
//});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseCors("AllowFrontend");  // **CORS Middleware**
//app.UseSession();
//app.UseMiddleware<TenantMiddleware>(); // **Tenant Middleware**
//app.UseStaticFiles();

//app.UseHttpsRedirection();

////app.UseSession();

//app.UseAuthorization();

//app.MapControllers();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowFrontend");

app.UseSession();

app.UseMiddleware<TenantMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
