
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Manager;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class SalesManManager : ISalesMan
    {
        readonly NTSoftDbContext _dbContext;
        private readonly NTSoftDbContextFactory _factory;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        private readonly string _connectionString;
        SQLConnectionString _connectionStringService = new SQLConnectionString();

        //public SalesManManager(FTDbContext dbContext, DapperService dapperService)
        //{
        //    _dbContext = dbContext;
        //    _IDapperService = dapperService;
        //    _ICommonService = new CommonService(_dbContext);
        //    _connectionString = _connectionStringService.GetConnectionString("default");
        //}

        public SalesManManager(NTSoftDbContext dbContext, NTSoftDbContextFactory factory, IDapperService dapperService)
        {
            _factory = factory;
            _dbContext = dbContext;
            _IDapperService = dapperService;
            //_ICommonService = new CommonService(_dbContext);
            _ICommonService = new CommonService(_factory);
            _connectionString = _connectionStringService.GetConnectionString("default");
        }
        public int Add(POStblSalesMan entity)
        {
            try
            {
                _ICommonService.Add<POStblSalesMan>(entity);
                return entity.Id;
            }
            catch
            {
                throw;
            }
        }

        public List<POStblSalesMan> GetAll()
        {
            try
            {
                return _ICommonService.GetAll<POStblSalesMan>().Where(x => x.IsActive == true).ToList();

            }
            catch
            {
                throw;
            }
        }

        public async Task<POStblSalesMan> GetById(int id)
        {
            var salesManList = await _dbContext.salasMan.FindAsync(id);
            return salesManList;
        }

        //public int Remove(POStblSalesMan entity)
        //{
        //    try
        //    {
        //        _ICommonService.RemoveById<POStblSalesMan>(entity.Id);

        //        return entity.Id;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public int Update(POStblSalesMan entity)
        {
            try
            {
                _ICommonService.Update<POStblSalesMan>(entity);
                return entity.Id;
            }
            catch
            {
                throw;
            }
        }

        public int Remove(POStblSalesMan entity)
        {
            throw new NotImplementedException();
        }
    }
}
