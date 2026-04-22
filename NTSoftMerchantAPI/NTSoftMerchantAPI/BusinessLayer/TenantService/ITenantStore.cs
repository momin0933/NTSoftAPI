using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTSoftMerchantAPI.BusinessLayer.TenantService
{
    public interface ITenantStore
    {
        Tenant GetTenant(string tenantId);
    }
}
