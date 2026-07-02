
using InventoryAccountsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Interface
{
    public interface IItem 
    {
        List<RptItem> GetAllItemListWithStock(int WHId, string type);
        List<RptItem> GetAllVariantList(int WHId);

        List<RptItem> GetAllItemListWithStockByStyleNo(string styleNo, int WhId);

        //List<RptSupplierWiseReport> GetSupplierWiseData(DateTime fromDate, DateTime toDate);

        public int DeleteById(int id);
        public string CountItem();
        public string GetItemNameById(int id);
        //public VariantListForDropdown? GetItemDetailsById(int id);

        //public int UpdateItemVariant(ItemVariantUpdate itemVariant);

        int AddVariantToExistingItem(List<ItemVariant> variants);

        int UpdateItemImage(int itemId, string imgPath, string updateBy);
    }
}
