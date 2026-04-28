using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using NTSoftMerchantAPI.BusinessLayer.Interface;
using NTSoftMerchantAPI.BusinessLayer.Service;
using NTSoftMerchantAPI.Models;
using System.Net.NetworkInformation;
using static Dapper.SqlMapper;

namespace NTSoftMerchantAPI.BusinessLayer.Manager
{
    public class OrderManager : IOrderManager
    {
        //private readonly NTSoftDbContext _dbContext;
        //private readonly IDapperService _dapperService;
        //private readonly ICommonService _commonService;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<OrderManager> _logger;
        readonly NTSoftDbContextFactory _dbContext;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        //private string _connectionString;

        public OrderManager(NTSoftDbContextFactory dbContext, IDapperService dapperService,ILogger<OrderManager> logger)
        {
            //_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            //_dapperService = dapperService ?? throw new ArgumentNullException(nameof(dapperService));
            //_commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));
            //_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            //_logger = logger ?? throw new ArgumentNullException(nameof(logger));

            //// Get connection string from tenant
            //var tenant = _httpContextAccessor.HttpContext?.Items["Tenant"] as TenantService.Tenant;
            //_connectionString = tenant?.ConnectionString ?? throw new InvalidOperationException("Connection string not found");
            _dbContext = dbContext;
            _IDapperService = dapperService;
            _ICommonService = new CommonService(_dbContext);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region GET Operations (Dapper)

        public IEnumerable<Order> GetAllOrders(int PageNumber, int PageSize)
        {
            try
            {
                string procedur = "SP_Order_GetAll";
                DynamicParameters p = new DynamicParameters();
                p.Add("@PageNumber", PageNumber);
                p.Add("@PageSize", PageSize);
                var UserList = _IDapperService.GetAllBySP<Order>(procedur, p).ToList();
                return UserList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                throw;
            }
        }

        public Order GetOrderById(int orderId)
        {
            try
            {
                string procedur = "SP_Order_GetById";
                DynamicParameters p = new DynamicParameters();
                p.Add("@OrderId", orderId);
                var UserList = _IDapperService.GetByDynamicSPSingle<Order>(procedur, p);
                return UserList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting order by id: {orderId}");
                throw;
            }
        }

        public IEnumerable<Order> GetOrdersByCustomer(int customerId)
        {
            try
            {
                string procedur = "SP_Order_GetByCustomer";
                DynamicParameters p = new DynamicParameters();
                p.Add("@CustomerId", customerId);
                var OrderList = _IDapperService.GetAllBySP<Order>(procedur, p).ToList();
                return OrderList;
            }
            catch (Exception ex)    
            {
                _logger.LogError(ex, $"Error getting orders by customer: {customerId}");
                throw;
            }
        }      

        public IEnumerable<Order> GetOrdersByBuyer(int buyerId)
        {
            try
            {          
                string procedur = "SP_Order_GetByBuyer";
                DynamicParameters p = new DynamicParameters();
                p.Add("@buyerId", buyerId);
                var OrderList = _IDapperService.GetAllBySP<Order>(procedur, p).ToList();
                return OrderList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting orders by buyer: {buyerId}");
                throw;
            }
        }

        public IEnumerable<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                string procedur = "SP_Order_GetByDateRange";
                DynamicParameters p = new DynamicParameters();
                p.Add("@BuyerShipFromDt", startDate);
                p.Add("@BuyerShipToDt", endDate);
                var OrderList = _IDapperService.GetAllBySP<Order>(procedur, p).ToList();
                return OrderList;               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting orders by date range: {startDate} - {endDate}");
                throw;
            }
        }

        #endregion

        #region INSERT Operations (Entity Framework)

        public async Task<int> AddOrderAsync(Order order)
        {
            try
            {
                var id = await _ICommonService.Add<Order>(order);
                _logger.LogInformation($"Order added successfully with Id: {order.Id}");
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding order");
                throw;
            }
        }

        

        #endregion

        #region UPDATE Operations (Entity Framework)

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            try
            {
                _ICommonService.Update<Order>(order);

                _logger.LogInformation($"Order updated successfully with Id: {order.Id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order with Id: {order.Id}");
                throw;
            }
        }

        #endregion

        #region DELETE Operations (Entity Framework)

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            try
            {
                if (orderId <= 0)
                {
                    _logger.LogWarning("Invalid OrderId: {OrderId}", orderId);
                    return false;
                }

                var isDeleted = _ICommonService.Remove<Order>(orderId);

                if (isDeleted <= 0)
                {
                    _logger.LogWarning("Order not found or not deleted. Id: {OrderId}", orderId);
                    return false;
                }

                _logger.LogInformation("Order deleted successfully with Id: {OrderId}", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order with Id: {OrderId}", orderId);
                throw;
            }
        }

        #endregion
    }
}