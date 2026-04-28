using NTSoftMerchantAPI.Models;

namespace NTSoftMerchantAPI.BusinessLayer.Interface
{
    public interface IOrderManager
    {
        // GET operations (Dapper)
        IEnumerable<Order> GetAllOrders(int PageNumber, int PageSize);
        IEnumerable<Order> GetOrdersByCustomer(int customerId);       
        IEnumerable<Order> GetOrdersByBuyer(int buyerId);
        Order GetOrderById(int orderId);
        IEnumerable<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate);

        // INSERT operations (Entity Framework)
        Task<int> AddOrderAsync(Order order);
       

        // UPDATE operations (Entity Framework)
        Task<bool> UpdateOrderAsync(Order order);

        // DELETE operations (Entity Framework)
        Task<bool> DeleteOrderAsync(int Id);
    }
}