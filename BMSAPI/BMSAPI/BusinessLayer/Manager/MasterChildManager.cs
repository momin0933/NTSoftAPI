using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using Dapper;

namespace BMSAPI.BusinessLayer.Manager
{
   
        public class MasterChildManager : IMasterChild
        {
            private readonly ILogger<MasterChildManager> _logger;
            private readonly IDapperService _IDapperService;
            private const string SP_NAME = "SP_MasterChild";

            public MasterChildManager(IDapperService dapperService, ILogger<MasterChildManager> logger)
            {
                _IDapperService = dapperService;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public IEnumerable<MasterChildItem> GetChildList(string? accessKey, int? parentId)
            {
                try
                {
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@QueryChecker", 1);
                    p.Add("@AccessKey", accessKey);
                    p.Add("@ParentId", parentId);

                    return _IDapperService.GetAllBySP<MasterChildItem>(SP_NAME, p).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting master child list for AccessKey: {AccessKey}", accessKey);
                    throw;
                }
            }
        }
}
