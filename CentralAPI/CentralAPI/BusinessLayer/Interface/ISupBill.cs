using CentralAPI.Models.ReportModels;

namespace CentralAPI.BusinessLayer.Interface
{
    public interface ISupBill
    {
        List<RptSubBill> GetSubBill(string projectName, DateTime invoiceDt);
    }
}
