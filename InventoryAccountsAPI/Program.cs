
using FTSoftAPI.Core.DataLayer.Repository;
using FTSoftAPI.Service.BusinessLayer.Manager;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Manager;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.BusinessLayer.TenantService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Newtonsoft.Json
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore;

        options.SerializerSettings.ContractResolver =
            new Newtonsoft.Json.Serialization.DefaultContractResolver();
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

// Tenant services
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<ITenantStore, TenantStore>();

// Database factory
builder.Services.AddScoped<NTSoftDbContextFactory>();
// Create tenant-specific DbContext using the factory
builder.Services.AddScoped<NTSoftDbContext>(provider =>
{
    var factory = provider.GetRequiredService<NTSoftDbContextFactory>();
    return factory.CreateDbContext();
});
// Dapper services
builder.Services.AddScoped<DapperService>();

builder.Services.AddScoped<IDapperService>(provider =>
    provider.GetRequiredService<DapperService>());
// Dapper and common services
//builder.Services.AddScoped<IDapperService, DapperService>();
builder.Services.AddScoped<ICommonService, CommonService>();

// Business services - existing
builder.Services.AddScoped<IItem, ItemManager>();
builder.Services.AddScoped<IMenu, MenuManager>();

// Additional services from Interface and Manager folders
builder.Services.AddScoped<IBankAccount, AccountManager>();
builder.Services.AddScoped<ICategory, CategoryManager>();
builder.Services.AddScoped<IDashboard, DashboardManager>();
builder.Services.AddScoped<IDiscount, DiscountManager>();
builder.Services.AddScoped<IInvoice, InvoiceManager>();
builder.Services.AddScoped<IItemReceive, ItemReceiveManager>();
builder.Services.AddScoped<IItemReturn, ItemReturnManager>();
builder.Services.AddScoped<IItemTransfer, ItemTransferManager>();
builder.Services.AddScoped<IProcedure, ProcedureManager>();
builder.Services.AddScoped<ISales, SalesManager>();
builder.Services.AddScoped<ISalesMan, SalesManManager>();
builder.Services.AddScoped<ISetting, SettingManager>();
builder.Services.AddScoped<IDropDown, DropDownManager>();
builder.Services.AddScoped<IAcctblVoucher, AcctblVoucherManager>();
builder.Services.AddScoped<IAccLevelTwo, AccLevelTwoManager>();
builder.Services.AddScoped<IModule, ModuleManager>();
// Authentication
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidIssuer = builder.Configuration["Jwt:Issuer"],

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        builder.Configuration["Jwt:Key"]!)),

                ClockSkew = TimeSpan.Zero
            };
    });

// CORS
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins ?? Array.Empty<string>())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
