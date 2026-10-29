
using InventoryAccountsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Interface
{
    public interface ISetting:ICommonMethod<StockAdjustment>
    {
        public int AddStockAdjustment(List<StockAdjustmentModel> entity);
        public int AddFinalAdjustment(List<FinalStockAdjustment> entity);
        public List<FinalStockAdjustment> GetAdjutmentItemWithActualStock( int WhId);
        public List<RptItem> GetVariantByBarcode(string barcode);
        public int InventoryAdjustment(InventoryAdj entity);


    }
}
