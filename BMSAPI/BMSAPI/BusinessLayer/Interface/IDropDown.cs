using BMSAPI.Models.Common;
using BMSAPI.Models;

namespace BMSAPI.BusinessLayer.Interface
{
    public interface IDropDown
    {
        List<Company> GetAllCompany();
        List<CustomerDropdown> GetAllCustomer();
        List<InvoiceDropdown> GetAllInvoice();
        List<ItemNameList> GetAllItemList();
        List<LevelOneIdList> GetAllLevelOneId();
        List<LevelOneDescription> GetAllLevelOneDescription();
        List<Ledger_Rpt_LevelTwo> GetAllLedgerRptLevelTwoData();
        List<Ledger_LevelThreeReport> Get_LedgerlLevelThreeData();
        List<ChartOfAccLevelOneIdList> GetAllChartOfAccLevelOneId();
        List<LevelTwoIdList> GetAllLevelTwoId();
       // List<SubLedgerDdl> GetAllLevelThreeIdDescrp();
        List<AccILedger> GetAllDataList(int LevelOneAutoID);
        List<MasterChild> GetMasterChildListByAccessKey(string AccessKey);
        List<Dropdown> GetTeamListByAccessKey(string AccessKey, int? UnitId);
        List<AccCodeLoad> GetAllAccCodeLoad();
        List<AccNameLoad> GetAllAccNameLoad();
        List<BrandsForDropDown> GetAllBrands();
        List<CategoryForDropDown> GetAllCategory();
        List<ItemsForDropDown> GetAllItemName();
        List<ItemsForDropDown> GetAllItemWithDesc();
        List<ItemNameList> GetItemsByWarehouse(int whId);
        List<AccLevelThree> GetAllSubAccList(int LevelTwoAutoId);
        string? LastVoucherNumber();
        List<string> GetAllColor();
        List<string> GetAllSize();
        List<Company> GetAllCompanyForAcc();
        List<string> GetAllVoucherNumberWithPrefix(string typeString);
        List<MasterChild> GetAllDepartment();
        List<MasterParent> GetAllPDepartment();
        List<MasterChild> GetAllDesignation();
        List<tbl_Company> Logo();
        List<ReceiveNoList> GetAllReceiveNo();
        List<tbl_Company> GetLogoByCompanyId(int compId);
        List<MasterChild> GetAllMaterialType();
        List<MasterChild> GetAllUnit();
    }
}
