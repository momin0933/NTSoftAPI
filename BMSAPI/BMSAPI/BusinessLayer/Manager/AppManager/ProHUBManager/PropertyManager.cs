using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models.Apps.PropHUB;
using Dapper;

namespace BMSAPI.BusinessLayer.Manager.AppManager.ProHUBManager
{
    public class PropertyManager : IProperty
    {
        private readonly ILogger<PropertyManager> _logger;
        private readonly IDapperService _IDapperService;
        private const string SP_NAME = "SP_Property";
        public PropertyManager(IDapperService dapperService, ILogger<PropertyManager> logger)
        {
            _IDapperService = dapperService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public bool AddProperty(Property model)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@Phone", model.Phone);
                p.Add("@Name", model.Name);
                p.Add("@Address", model.Address);
                p.Add("@SecurityName", model.SecurityName);
                p.Add("@SecurityPhone", model.SecurityPhone);
                p.Add("@Type", model.Type);
                p.Add("@Remarks", model.Remarks);
                p.Add("@EntryBy", model.EntryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int affectedRows = (int)result.AffectedRows;

                if (affectedRows <= 0)
                {
                    _logger.LogWarning("Add property failed for Name: {Name}", model.Name);
                    return false;
                }

                _logger.LogInformation("Property added successfully with Id: {Id}, Name: {Name}", (int)result.Id, model.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding property with Name: {Name}", model.Name);
                throw;
            }
        }

        public IEnumerable<Property> GetPropertyList()
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 2);

                return _IDapperService.GetAllBySP<Property>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting property list");
                throw;
            }
        }

        public bool AddPropertyDetails(PropertyDetails model)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 3);
                p.Add("@Phone", model.Phone);
                p.Add("@PropertyId", model.PropertyId);
                p.Add("@FlatName", model.FlatName);
                p.Add("@Floor", model.Floor);
                p.Add("@Room", model.Room);
                p.Add("@Bathroom", model.Bathroom);
                p.Add("@Balcony", model.Balcony);
                p.Add("@MeterNo", model.MeterNo);
                p.Add("@Remarks", model.Remarks);
                p.Add("@EntryBy", model.EntryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int affectedRows = (int)result.AffectedRows;

                if (affectedRows <= 0)
                {
                    _logger.LogWarning("Add property details failed for PropertyId: {PropertyId}", model.PropertyId);
                    return false;
                }

                _logger.LogInformation("Property details added successfully with Id: {Id}, PropertyId: {PropertyId}", (int)result.Id, model.PropertyId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding property details for PropertyId: {PropertyId}", model.PropertyId);
                throw;
            }
        }
    }
}
