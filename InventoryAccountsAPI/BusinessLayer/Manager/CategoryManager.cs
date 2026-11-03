
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class CategoryManager : ICategory
    {
        readonly NTSoftDbContext _dbContext;
        private readonly NTSoftDbContextFactory _factory;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        private readonly string _connectionString;
        SQLConnectionString _connectionStringService = new SQLConnectionString();
        public CategoryManager(NTSoftDbContext dbContext, NTSoftDbContextFactory factory, DapperService dapperService)
        {
            _factory = factory;
            _dbContext = dbContext;
            _IDapperService = dapperService;
            //_ICommonService = new CommonService(_dbContext);
            _ICommonService = new CommonService(_factory);
            _connectionString = _connectionStringService.GetConnectionString("default");
        }
        public int Add(POStblCategory entity)
        {
            try
            {
                _ICommonService.Add<POStblCategory>(entity);
                return entity.Id;
            }
            catch
            {
                throw;
            }
        }

        public List<POStblCategory> GetAll()
        {
            try
            {
                return _ICommonService.GetAll<POStblCategory>().Where(x => x.IsActive == true).ToList();

            }
            catch
            {
                throw;
            }
        }

        public int Remove(POStblCategory entity)
        {
            try
            {
                _ICommonService.RemoveById<POStblCategory>(entity.Id);

                return entity.Id;
            }
            catch
            {
                throw;
            }
        }

        public int Update(POStblCategory entity)
        {
            try
            {
                _ICommonService.Update<POStblCategory>(entity);
                return entity.Id;
            }
            catch
            {
                throw;
            }
        }

        public async Task<POStblCategory> GetById(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            return category;
        }

       
    }
}
