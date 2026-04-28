using NTSoftMerchantAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTSoftMerchantAPI.BusinessLayer.TenantService
{
    public class Tenant:Base
    {
        //public string Id { get; set; } // Unique identifier for the tenant
        public string TenantKey { get; set; }
        public string ConnectionString { get; set; }
    }
}
