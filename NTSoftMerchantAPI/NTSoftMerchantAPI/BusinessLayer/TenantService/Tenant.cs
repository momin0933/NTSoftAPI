using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTSoftMerchantAPI.BusinessLayer.TenantService
{
    public class Tenant
    {
        public string Id { get; set; } // Unique identifier for the tenant
        public string Name { get; set; }
        public string ConnectionString { get; set; }
    }
}
