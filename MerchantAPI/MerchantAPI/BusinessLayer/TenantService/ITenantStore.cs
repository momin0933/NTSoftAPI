using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerchantAPI.BusinessLayer.TenantService
{
    public interface ITenantStore
    {
        Tenant GetTenant(string tenantId);
    }
}
