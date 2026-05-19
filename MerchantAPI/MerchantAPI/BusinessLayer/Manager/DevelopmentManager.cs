using Dapper;
using Microsoft.Extensions.Logging;
using MerchantAPI.BusinessLayer.Interface;
using MerchantAPI.BusinessLayer.Service;
using MerchantAPI.Models;

namespace MerchantAPI.BusinessLayer.Manager
{
    public class DevelopmentManager : IDevelopmentManager
    {
        private readonly ILogger<DevelopmentManager> _logger;
        readonly NTSoftDbContextFactory _dbContext;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;

        private const string SP_NAME = "SP_Development";

        public DevelopmentManager(NTSoftDbContextFactory dbContext, IDapperService dapperService, ILogger<DevelopmentManager> logger)
        {
            _dbContext = dbContext;
            _IDapperService = dapperService;
            _ICommonService = new CommonService(_dbContext);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region GET Operations (Dapper)

        // Action = 1 → Get All Paginated
        public IEnumerable<RptDevelopment> GetAllDevelopments(int PageNumber, int PageSize)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@Action", 1);
                p.Add("@PageNumber", PageNumber);
                p.Add("@PageSize", PageSize);

                return _IDapperService.GetAllBySP<RptDevelopment>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all developments");
                throw;
            }
        }

        // Action = 2 → Get By Id
        public Development GetDevelopmentById(int developmentId)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@Action", 2);
                p.Add("@DevelopmentId", developmentId);

                return _IDapperService.GetByDynamicSPSingle<Development>(SP_NAME, p);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting development by id: {developmentId}");
                throw;
            }
        }

        // Action = 3 → Get By Buyer
        public IEnumerable<Development> GetDevelopmentsByBuyer(int buyerId)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@Action", 3);
                p.Add("@BuyerId", buyerId);

                return _IDapperService.GetAllBySP<Development>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting developments by buyer: {buyerId}");
                throw;
            }
        }

        // Action = 4 → Get By Customer
        public IEnumerable<Development> GetDevelopmentsByCustomer(int customerId)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@Action", 4);
                p.Add("@CustomerId", customerId);

                return _IDapperService.GetAllBySP<Development>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting developments by customer: {customerId}");
                throw;
            }
        }

        // Action = 5 → Get By Factory
        public IEnumerable<Development> GetDevelopmentsByFactory(int factoryId)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@Action", 5);
                p.Add("@FactoryId", factoryId);

                return _IDapperService.GetAllBySP<Development>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting developments by factory: {factoryId}");
                throw;
            }
        }

        // Action = 6 → Get By Date Range
        public IEnumerable<Development> GetDevelopmentsByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@Action", 6);
                p.Add("@FromDate", startDate);
                p.Add("@ToDate", endDate);

                return _IDapperService.GetAllBySP<Development>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting developments by date range: {startDate} - {endDate}");
                throw;
            }
        }

        #endregion

        #region INSERT Operation (Dapper) — Action = 7

        public async Task<int> AddDevelopmentAsync(Development development)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@Action", 7);
                p.Add("@BuyerId", development.BuyerId);
                p.Add("@CustomerId", development.CustomerId);
                p.Add("@FactoryId", development.FactoryId);
                p.Add("@OrderSeasonId", development.OrderSeasonId);
                p.Add("@OrderTypeId", development.OrderTypeId);
                p.Add("@DepartmentId", development.DepartmentId);
                p.Add("@CategoryId", development.CategoryId);
                p.Add("@InqDate", development.InqDate);
                p.Add("@PurchaseOrder", development.PurchaseOrder);
                p.Add("@StyleNo", development.StyleNo);
                p.Add("@Description", development.Description);
                p.Add("@FabricDescription", development.FabricDescription);
                p.Add("@DestinationId", development.DestinationId);
                p.Add("@TotalOrderQty", development.TotalOrderQty);
                p.Add("@OfferPrice", development.OfferPrice);
                p.Add("@ImagePath", development.ImagePath);
                p.Add("@ImageName", development.ImageName);
                p.Add("@Remarks", development.Remarks);
                p.Add("@EntryBy", development.EntryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int newId = (int)result.Id;

                _logger.LogInformation($"Development added successfully with Id: {newId}");
                return newId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding development");
                throw;
            }
        }

        #endregion

        #region UPDATE Operation (Dapper) — Action = 8

        public async Task<bool> UpdateDevelopmentAsync(Development development)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@Action", 8);
                p.Add("@DevelopmentId", development.Id);
                p.Add("@BuyerId", development.BuyerId);
                p.Add("@CustomerId", development.CustomerId);
                p.Add("@FactoryId", development.FactoryId);
                p.Add("@OrderSeasonId", development.OrderSeasonId);
                p.Add("@OrderTypeId", development.OrderTypeId);
                p.Add("@DepartmentId", development.DepartmentId);
                p.Add("@CategoryId", development.CategoryId);
                p.Add("@InqDate", development.InqDate);
                p.Add("@PurchaseOrder", development.PurchaseOrder);
                p.Add("@StyleNo", development.StyleNo);
                p.Add("@Description", development.Description);
                p.Add("@FabricDescription", development.FabricDescription);
                p.Add("@DestinationId", development.DestinationId);
                p.Add("@TotalOrderQty", development.TotalOrderQty);
                p.Add("@OfferPrice", development.OfferPrice);
                p.Add("@ImagePath", development.ImagePath);
                p.Add("@ImageName", development.ImageName);
                p.Add("@Remarks", development.Remarks);
                p.Add("@UpdateBy", development.UpdateBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int affectedRows = (int)result.AffectedRows;

                if (affectedRows <= 0)
                {
                    _logger.LogWarning($"Development not found or not updated. Id: {development.Id}");
                    return false;
                }

                _logger.LogInformation($"Development updated successfully with Id: {development.Id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating development with Id: {development.Id}");
                throw;
            }
        }

        #endregion

        #region DELETE Operation (Dapper) — Action = 9

        public async Task<bool> DeleteDevelopmentAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid DevelopmentId: {DevelopmentId}", id);
                    return false;
                }

                DynamicParameters p = new DynamicParameters();
                p.Add("@Action", 9);
                p.Add("@DevelopmentId", id);
                p.Add("@UpdateBy", "System");

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int affectedRows = (int)result.AffectedRows;

                if (affectedRows <= 0)
                {
                    _logger.LogWarning("Development not found or not deleted. Id: {DevelopmentId}", id);
                    return false;
                }

                _logger.LogInformation("Development deleted successfully with Id: {DevelopmentId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting development with Id: {DevelopmentId}", id);
                throw;
            }
        }

        #endregion
    }
}