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


        public IEnumerable<PropertyDetails> GetPropertyDetailsList(int propertyId)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 4);
                p.Add("@PropertyId", propertyId);

                return _IDapperService.GetAllBySP<PropertyDetails>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting property details list for PropertyId: {PropertyId}", propertyId);
                throw;
            }
        }

        public IEnumerable<Property> GetMyPropertyList(string phone)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 5);
                p.Add("@Phone", phone);

                return _IDapperService.GetAllBySP<Property>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting my property list for Phone: {Phone}", phone);
                throw;
            }
        }
        public IEnumerable<PropertyDetailsFullView> GetPropertyDetailsFullList(int propertyId)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 6);
                p.Add("@PropertyId", propertyId);

                return _IDapperService.GetAllBySP<PropertyDetailsFullView>(SP_NAME, p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting full property details list for PropertyId: {PropertyId}", propertyId);
                throw;
            }
        }

        public bool ToggleActiveStatus(int propertyId, bool isActive, string phone, string entryBy)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 7);
                p.Add("@PropertyId", propertyId);
                p.Add("@IsActive", isActive);
                p.Add("@Phone", phone);
                p.Add("@EntryBy", entryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int affectedRows = (int)result.AffectedRows;

                if (affectedRows <= 0)
                {
                    _logger.LogWarning("Toggle active status failed for PropertyId: {PropertyId}", propertyId);
                    return false;
                }

                _logger.LogInformation("Property {PropertyId} active status set to {IsActive}", propertyId, isActive);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling active status for PropertyId: {PropertyId}", propertyId);
                throw;
            }
        }
    }
}
