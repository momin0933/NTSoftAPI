using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NTSoftMerchantAPI.BusinessLayer.TenantService
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private HttpContext _httpContext;
        private readonly ILogger<TenantMiddleware> _logger;

        public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }   
        public async Task InvokeAsync(HttpContext context, ITenantStore tenantStore)
        {
            var tenantId = context.Request.Headers["TenantId"].FirstOrDefault();
            if (!string.IsNullOrEmpty(tenantId))
            {
                var tenant = tenantStore.GetTenant(tenantId);
                if (tenant != null)
                {
                    context.Items["Tenant"] = tenant;
                }
            }

            await _next(context);
        }
    }
}
