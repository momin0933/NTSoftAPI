
using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface IGas: ICommonMethod<Gas>
    {
        public List<RptGas> GetGasWithSupplier(DateTime fromDt, DateTime toDt);
        public List<Supplier> GetSuppliers();

        public List<Gas> GetGasById(int id);
        public List<RptGasUsage> GetGasUsage(string FCode);
        public int AddUsedGas(GasUsed entity);
        public int UpdateUsedGas(GasUsed entity);
        public List<RptGasStock> GetGasStock(DateTime fromDate, DateTime toDate);
        public int AddGasStock(GasStock entity);
        public int UpdateGasStock(GasStock entity);
        public GasStockCal GetGasOpeningStock(int buildingId);
        public List<RptGasUsage> GetAllFlatInfo();

        public int AddGasUsed(List<GasUsed> entity);
        public List<RptGasUsage> GetGasUsageList();
        public int AddClosingDate(ClosingDeclaration entity);
        public List<ClosingDeclaration> GetClosingDate();
        public int UpdateClosingDate(ClosingDeclaration entity);


    }
}
