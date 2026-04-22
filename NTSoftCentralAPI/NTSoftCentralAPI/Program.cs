

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using NTSoftCentralAPI.BusinessLayer.Interface;
using NTSoftCentralAPI.BusinessLayer.Manager;
using NTSoftCentralAPI.BusinessLayer.Service;
using NTSoftCentralAPI.BusinessLayer.TenantService;
using System.Text;

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
builder.Services.AddScoped<NTSoftDbContextFactory>();
builder.Services.AddTransient<DapperService>();
builder.Services.AddMemoryCache(); // Singleton হিসেবে ব্যবহার করুন

//builder.Services.AddTransient<SQLConnectionString>();
//builder.Services.AddScoped<SQLConnectionString>();
builder.Services.AddScoped<IUser, UserManager>();
builder.Services.AddScoped<ITenantStore, TenantStore>();
builder.Services.AddScoped<CustomService>();



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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
         "http://localhost:3000",
         "http://localhost:3001",
         "http://localhost:3002",
         "http://localhost:3003",
         "http://localhost:3004",
         "http://localhost:3005",
         "https://my.minglefashionbd.com",
         "http://my.minglefashionbd.com",
         "https://vms.minglefashionbd.com",
         "http://vms.minglefashionbd.com",
         "https://minglefashionbd.com",
         "http://minglefashionbd.com",
         "https://smart-hrm.vercel.app/",
         "https://ecom.minglefashionbd.com",
         "http://ecom.minglefashionbd.com",
         "http://144.91.82.79:3000",
         "https://144.91.82.79:3000",
         "https://rfosbd.com",
         "http://rfosbd.com",
         "https://ntsoftbd.com",
         "http://ntsoftbd.com",
         "https://exam.minglefashionbd.com",
         "http://exam.minglefashionbd.com",
         "https://bms.ntsoftbd.com",
         "http://bms.ntsoftbd.com",
         "http://hr.minglefashionbd.com",
         "https://hr.minglefashionbd.com",
         "https://mingleinstylebd.com",
         "http://mingleinstylebd.com",
         "http://116.68.192.10:5000",
         "https://hr.ntsoftbd.com",
         "http://hr.ntsoftbd.com"

)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");  // **CORS Middleware**
app.UseMiddleware<TenantMiddleware>(); // **Tenant Middleware**
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
