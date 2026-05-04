using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMSAPI.BusinessLayer.TenantService
{
    public interface ITenantStore
    {
        Tenant GetTenant(string tenantId);
    }
}
