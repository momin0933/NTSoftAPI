using Dapper;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class ProcedureManager : IProcedure
    {

        readonly NTSoftDbContext _dbContext;
        readonly NTSoftDbContextFactory _factory;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        public ProcedureManager(NTSoftDbContext dbContext, NTSoftDbContextFactory factory, DapperService dapperService)
        {
            _factory = factory;
            _dbContext = dbContext;
            _IDapperService = dapperService;
            //_ICommonService = new CommonService(_dbContext);
            _ICommonService = new CommonService(_factory);
        }
        public async Task<IEnumerable<dynamic>> CallProcedureAsync1(ProcedureExecuteRequest request)
        {
            var parameters = new DynamicParameters();

            foreach (var param in request.Parameters)
            {
                parameters.Add($"@{param.Key}", param.Value);
            }

            try
            {
                var data = await _IDapperService.CallProcedureAsync(request.ProcedureName, parameters);
                return data;
            }
            catch (Exception ex)
            {
                //return "Invalid operation type. Allowed: READ, INSERT, UPDATE, DELETE.";
            }
            return Enumerable.Empty<dynamic>();
        }
        public async Task<int> ExecuteProcedureNonQueryAsync(ProcedureExecuteRequest request)
        {
            var parameters = new DynamicParameters();

            foreach (var param in request.Parameters)
            {
                parameters.Add($"@{param.Key}", param.Value);
            }

            try
            {
                int affectedRows = await _IDapperService.ExecuteProcedureNonQueryAsync(request.ProcedureName, parameters);
                return affectedRows;
            }
            catch (Exception ex)
            {
                //return "Invalid operation type. Allowed: READ, INSERT, UPDATE, DELETE.";
            }
            return 0;
        }
        //public async Task<IEnumerable<dynamic>> CallProcedureAsync(string procedureName, DynamicParameters parameters)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    await connection.OpenAsync();

        //    var result = await connection.QueryAsync(
        //        sql: procedureName,
        //        param: parameters,
        //        commandType: CommandType.StoredProcedure
        //    );

        //    return result;
        //}

        //public async Task<int> ExecuteProcedureNonQueryAsync(string procedureName, DynamicParameters parameters)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    await connection.OpenAsync();

        //    var affectedRows = await connection.ExecuteAsync(
        //        sql: procedureName,
        //        param: parameters,
        //        commandType: CommandType.StoredProcedure
        //    );

        //    return affectedRows;
        //}
    }
}
