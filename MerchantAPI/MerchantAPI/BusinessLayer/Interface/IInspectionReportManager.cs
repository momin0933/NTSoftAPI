using MerchantAPI.Models.Inspections;

namespace MerchantAPI.BusinessLayer.Interface
{
    public interface IInspectionReportManager
    {
        Task<(List<InspectionReport> Data, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, DateTime? fromDate, DateTime? toDate, string? search);
        Task<InspectionReport?> GetByIdAsync(int id);
        Task<int> AddAsync(InspectionReport report, string? userName);
        Task<bool> UpdateAsync(InspectionReport report, string? userName);
        Task<bool> DeleteAsync(int id, string? userName);
    }
}
