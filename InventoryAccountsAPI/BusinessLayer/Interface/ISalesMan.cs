
using InventoryAccountsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Interface
{
    public interface ISalesMan: ICommonMethod<POStblSalesMan>
    {
        public Task<POStblSalesMan> GetById(int id);
    }
}
