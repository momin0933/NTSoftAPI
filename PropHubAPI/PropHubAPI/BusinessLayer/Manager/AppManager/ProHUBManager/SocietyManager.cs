using Dapper;
using PropHubAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using PropHubAPI.BusinessLayer.Service;

namespace PropHubAPI.BusinessLayer.Manager.AppManager.ProHUBManager
{
    public class SocietyManager : ISociety
    {
        private readonly ILogger<SocietyManager> _logger;
        private readonly IDapperService _IDapperService;
        private const string SP_NAME = "SP_Society";

        public SocietyManager(IDapperService dapperService, ILogger<SocietyManager> logger)
        {
            _IDapperService = dapperService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<SocietyItem> GetSocietyList()
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);

                return _IDapperService.GetAllBySP<SocietyItem>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting society list");
                throw;
            }
        }
    }
}
