
using InventoryAccountsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Interface
{
    public interface IItemReturn
    {
        public List<POSItemReturn> GetAll();
        public POSItemReturn GetById(int Id);
        public int Add(POSItemReturn entity);
        public int Update(POSItemReturn entity);
        //public int Delete(int id);
        List<RptItem> GetAllItemStockListWithPrice(int WHId, string type);
        public int CountReturnInvoice(string compCode);
    }
}
