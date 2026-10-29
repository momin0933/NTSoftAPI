using Dapper;
using InventoryAccountsAPI.Models;


namespace InventoryAccountsAPI.BusinessLayer.Interface
{
    public interface IProcedure
    {
        Task<IEnumerable<dynamic>> CallProcedureAsync1(ProcedureExecuteRequest request);
        Task<int> ExecuteProcedureNonQueryAsync(ProcedureExecuteRequest request);
        //Task<int> ExecuteProcedureNonQueryAsync(string procedureName, DynamicParameters parameters);
    }
}
