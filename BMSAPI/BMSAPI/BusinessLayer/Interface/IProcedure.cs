using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface IProcedure
    {
        Task<IEnumerable<dynamic>> CallProcedureAsync1(ProcedureExecuteRequest request);
        Task<int> ExecuteProcedureNonQueryAsync(ProcedureExecuteRequest request);
    }
}
