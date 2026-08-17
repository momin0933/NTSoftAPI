using BMSAPI.Models.Apps.PropHUB;

namespace BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB
{
    public interface IProperty
    {
        bool AddProperty(Property model);
        IEnumerable<Property> GetPropertyList();
        bool AddPropertyDetails(PropertyDetails model);
    }
}
