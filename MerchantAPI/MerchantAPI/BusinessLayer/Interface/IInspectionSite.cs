using MerchantAPI.Models.Inspections;

namespace MerchantAPI.BusinessLayer.Interface
{
    public interface IInspectionSite
    {
        Task<List<InspectionSite>> GetAllAsync(string? search);
        Task<InspectionSite?> GetByIdAsync(int id);
        Task<int> AddAsync(InspectionSite site, string? userName);
        Task<bool> UpdateAsync(InspectionSite site, string? userName);
        Task<bool> DeleteAsync(int id, string? userName);
    }
}
