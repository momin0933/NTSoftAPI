using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTSoftMerchantAPI.BusinessLayer.TenantService
{
    public class TenantStore : ITenantStore
    {
        private readonly List<Tenant> _tenants = new()
        {   
            //Live CS
            new Tenant { Id = "POS0933", Name = "Tenant 1", ConnectionString = "Data Source=194.233.70.10;Initial Catalog=FashionTexLive;Uid = admin; Password = admin#@0722; TrustServerCertificate=True;" },
           
            new Tenant { Id = "AC0933", Name = "Tenant 2", ConnectionString = "Data Source=194.233.70.10;Initial Catalog=FashionTexAC;Uid = admin; Password = admin#@0722; Integrated Security=False;Trusted_Connection=False;" },
            
            new Tenant { Id = "Torana0933", Name = "Tenant 3", ConnectionString = "Data Source=194.233.70.10;Initial Catalog=ToranaFashionLive;Uid = admin; Password = admin#@0722; Integrated Security=False;Trusted_Connection=False;" },
            
            new Tenant { Id = "test", Name = "Tenant 4", ConnectionString = "Data Source=194.233.70.10;Initial Catalog=LeftOverTest;Uid = admin; Password = admin#@0722; Integrated Security=False;Trusted_Connection=False;" },
            
            new Tenant { Id = "Billing", Name = "Tenant 5", ConnectionString = "Data Source=194.233.70.10;Initial Catalog=CentralDB;Uid = admin; Password = admin#@0722; Integrated Security=False;Trusted_Connection=False;" },            
            
            new Tenant { Id = "VMS", Name = "Tenant 6", ConnectionString = "Data Source=194.233.70.10;Initial Catalog=VobonLive;Uid = admin; Password = admin#@0722; Integrated Security=False;Trusted_Connection=False;" },
            
            new Tenant { Id = "VMSTest", Name = "Tenant 7", ConnectionString = "Data Source=194.233.70.10;Initial Catalog=VobonTest;Uid = admin; Password = admin#@0722; Integrated Security=False;Trusted_Connection=False;" },


            new Tenant { Id = "CDI0933", Name = "Tenant 8", ConnectionString = "Data Source=194.233.70.10;Initial Catalog=StorePro_CDIDB;Uid = admin; Password = admin#@0722; Integrated Security=False;Trusted_Connection=False;" },
           
            new Tenant { Id = "NS0933", Name = "Tenant 9", ConnectionString = "Data Source=194.233.70.10;Initial Catalog=StorePro_NewShopDB;Uid = admin; Password = admin#@0722; Integrated Security=False;Trusted_Connection=False;" },


            new Tenant { Id = "FtexExam", Name = "Tenant 10", ConnectionString = "Data Source=194.233.70.10;Initial Catalog=ExamSystem;Uid = admin; Password = admin#@0722; Integrated Security=False;Trusted_Connection=False;" },
            

            new Tenant { Id = "Demo0933", Name = "Tenant 11", ConnectionString = "Data Source=194.233.70.10;Initial Catalog=StorePro_DemoDB;Uid = admin; Password = admin#@0722; Integrated Security=False;Trusted_Connection=False;" },

            new Tenant { Id = "AFM0933", Name = "Tenant 12", ConnectionString = "Data Source=194.233.70.10;Initial Catalog=StorePro_AFM;Uid = admin; Password = admin#@0722; Integrated Security=False;Trusted_Connection=False;" },

            new Tenant { Id = "SC0933", Name = "Tenant 13", ConnectionString = "Data Source=194.233.70.10;Initial Catalog=BMS_ShamolChaya;Uid = admin; Password = admin#@0722; Integrated Security=False;Trusted_Connection=False;" },

            new Tenant { Id = "HR0933", Name = "Tenant 14", ConnectionString = "Data Source=194.233.70.10;Initial Catalog=HRManagerLive;Uid = admin; Password = admin#@0722; Integrated Security=False;Trusted_Connection=False;" },

            
               // Local CS            
            new Tenant { Id = "kamil", Name = "Tenant 1", ConnectionString = "DESKTOP-MDESTFC\\SQLEXPRESS;Initial Catalog=VobonLive;Trusted_Connection=True;" },
            
           // new Tenant { Id = "123", Name = "Tenant 1", ConnectionString = "Data Source=DESKTOP-MDESTFC\\SQLEXPRESS;Initial Catalog=CentralDB;Trusted_Connection=True;" },

            new Tenant { Id = "Habib", Name = "Tenant 1", ConnectionString = "Data Source=FTEX-PC-156\\SQLEXPRESS;Initial Catalog=FashionTexLive;Trusted_Connection=True;" },

             new Tenant { Id = "Habib2", Name = "Tenant 1", ConnectionString = "Data Source=FTEX-PC-156\\SQLEXPRESS;Initial Catalog=StorePro_CDIDB;Trusted_Connection=True;" },

            new Tenant { Id = "Exam", Name = "Tenant 5", ConnectionString = "Data Source=FTEX-PC-156\\SQLEXPRESS;Initial Catalog=ExamSystem;Trusted_Connection=True;" },

            new Tenant { Id = "123", Name = "Tenant 5", ConnectionString = "Data Source=FTEX-PC-156\\SQLEXPRESS;Initial Catalog=CentralDB;Trusted_Connection=True;" },


           

            
        };

        public Tenant GetTenant(string tenantId)
        {
            return _tenants.FirstOrDefault(t => t.Id == tenantId);
        }
    }
}
