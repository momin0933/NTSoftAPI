using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTSoftCentralAPI.BusinessLayer.Service
{
    public interface IDapperService
    {
        IEnumerable<T> GetAllByQuery<T>(string query) where T : class;
        string GetStringByQuery(string query);
        IEnumerable<T> GetAllBySP<T>(string procedure, DynamicParameters p) where T : class;
        T GetByDynamicSPSingle<T>(string procedure, DynamicParameters p) where T : class;
        int Post(string query);
        bool PostBulkInsert<T>(IEnumerable<T> items, string tableName);
        int UpdateByQuery(string query);
        int UpdateByquery(string query1);
        void GetByMultipleQueryResult(string query, out string ReqQty, out string ActQty);
        int PostBySP(string procedure, DynamicParameters p);
        // In your DapperService implementation
        public  Task ExecuteAsync(string sql, object parameters = null);

        Task<IEnumerable<dynamic>> CallProcedureAsync(string procedureName, DynamicParameters parameters);
        Task<int> ExecuteProcedureNonQueryAsync(string procedureName, DynamicParameters parameters);

    }
}
