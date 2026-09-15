using BMSAPI.Models.Apps.PropHUB;

namespace BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface IProperty
    {
        bool AddProperty(Property model);
        IEnumerable<Property> GetPropertyList(int uId);
        bool AddPropertyDetails(PropertyDetails model);
        IEnumerable<PropertyDetails> GetPropertyDetailsList(int propertyId);
        IEnumerable<Property> GetMyPropertyList(string phone, int uId);
        IEnumerable<PropertyDetailsFullView> GetPropertyDetailsFullList(int propertyId);
        bool ToggleActiveStatus(int propertyId, bool isActive, string phone, int uId, string entryBy);
        bool DeletePropertyDetails(int propDetailsId, string phone, int uId, string entryBy);
    }
}