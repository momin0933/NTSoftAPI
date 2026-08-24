using BMSAPI.Models.Apps.PropHUB;

namespace BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface IProperty
    {
        bool AddProperty(Property model);
        IEnumerable<Property> GetPropertyList();
        bool AddPropertyDetails(PropertyDetails model);
        IEnumerable<PropertyDetails> GetPropertyDetailsList(int propertyId);
        IEnumerable<Property> GetMyPropertyList(string phone);
        IEnumerable<PropertyDetailsFullView> GetPropertyDetailsFullList(int propertyId);
        bool ToggleActiveStatus(int propertyId, bool isActive, string phone, string entryBy);
    }
}
