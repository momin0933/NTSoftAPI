using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTSoftAPI.Service.BusinessLayer.Service
{
    public class SQLConnectionString
    {
        public string ProjectCS { get; set; }
        public string ApiKey { get; set; }
        public string GetConnectionString(string ProjectCsKey)
        {
            string CS = "";
            if (ProjectCsKey == "default")
            {
               // CS = "Data Source=DESKTOP-MDESTFC\\SQLEXPRESS;Initial Catalog=FashionTexLive;Trusted_Connection=True;";
                //CS = "Data Source=194.233.70.10;Initial Catalog=FashionTexLive;Uid = admin; Password = admin#@0722; Integrated Security=False;Trusted_Connection=False;";
             //   CS = "Data Source=194.233.70.10;Initial Catalog=BetaPos;Uid = admin; Password = admin#@0722; Integrated Security=False;Trusted_Connection=False;";

            }
            return CS;
        }
        //public List<Tenant> GetConnectionStringList()
        //{
        //    List<Tenant> tenantList = new List<Tenant>();
        //    Tenant tenant = new Tenant();

        //    tenant.TID = "default";
        //    tenant.Name = "default";
        //    tenant.ConnectionString = "Data Source=172.20.3.152\\MSSQL2017;Initial Catalog=NNGAccounts;Uid = sa; Password = aA@01737918236;";
        //    tenantList.Add(tenant);

        //    tenant = new Tenant();
        //    tenant.TID = "CSNMLProperty2122";
        //    tenant.Name = "CSNMLProperty2122";
        //    tenant.ConnectionString = "Data Source=172.20.3.152\\MSSQL2017;Initial Catalog=NMLProperty2223;Uid = sa; Password = aA@01737918236;";
        //    tenantList.Add(tenant);

        //    return tenantList;

        //}
    }
    //public class Tenant
    //{
    //    public string Name { get; set; }
    //    public string TID { get; set; }
    //    public string ConnectionString { get; set; }
    //}
}
