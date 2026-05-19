using MerchantAPI.Models;

namespace MerchantAPI.BusinessLayer.Interface
{
    public interface IDevelopmentManager
    {
        // GET operations (Dapper)
        IEnumerable<RptDevelopment> GetAllDevelopments(int PageNumber, int PageSize);
        IEnumerable<Development> GetDevelopmentsByBuyer(int buyerId);
        IEnumerable<Development> GetDevelopmentsByCustomer(int customerId);
        IEnumerable<Development> GetDevelopmentsByFactory(int factoryId);
        IEnumerable<Development> GetDevelopmentsByDateRange(DateTime startDate, DateTime endDate);
        Development GetDevelopmentById(int developmentId);

        // INSERT operations (Entity Framework)
        Task<int> AddDevelopmentAsync(Development development);

        // UPDATE operations (Entity Framework)
        Task<bool> UpdateDevelopmentAsync(Development development);

        // DELETE operations (Entity Framework)
        Task<bool> DeleteDevelopmentAsync(int id);
    }
}