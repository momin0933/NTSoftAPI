using CentralAPI.Models;

namespace CentralAPI.BusinessLayer.Interface
{
    public interface IDropdown
    {
        List<BuyerDropdown> GetAllBuyers();
        List<CategoryDropdown> GetAllCategories();
        List<CustomerDropdown> GetAllCustomers();
        List<DepartmentDropdown> GetAllDepartments();
        List<FactoryDropdown> GetAllFactories();
        List<OrderSeasonDropdown> GetAllOrderSeasons();
        List<OrderTypeDropdown> GetAllOrderTypes();
        List<ShipmentTermsDropdown> GetAllShipmentTerms();
        List<DestinationDropdown> GetAllDestinations();
        List<ShippingModeDropdown> GetAllShippingModes();
        List<PaymentModeDropdown> GetAllPaymentModes();
    }
}
