using BMSAPI.BusinessLayer.Interface;
using BMSAPI.Models.Common;
using BMSAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers.Accounts
{
    [Route("api/[controller]")]
    [ApiController]
    public class DropDownController : ControllerBase
    {
        private readonly IDropDown _dropdown;

        public DropDownController(IDropDown dropdown)
        {
            _dropdown = dropdown;
        }

        [HttpGet("GetMasterChildList")]
        public List<MasterChild> GetMasterChildList(string AccessKey) => _dropdown.GetMasterChildListByAccessKey(AccessKey);

        [HttpGet("GetUnitTeamList")]
        public List<Dropdown> GetUnitAndTeamList(string AccessKey, int? UnitId) => _dropdown.GetTeamListByAccessKey(AccessKey, UnitId);

        [HttpGet("GetCompanyList")]
        public List<Company> GetCompanyList() => _dropdown.GetAllCompany();

        [HttpGet("GetCustomerList")]
        public List<CustomerDropdown> GetCustomerList() => _dropdown.GetAllCustomer();

        [HttpGet("GetInvoiceList")]
        public List<InvoiceDropdown> GetInvoiceList() => _dropdown.GetAllInvoice();

        [HttpGet("GetItemNameList")]
        public List<ItemNameList> GetItemNameList() => _dropdown.GetAllItemList();

        [HttpGet("GetAllLevelOneIdList")]
        public List<LevelOneIdList> GetAllLevelOneIdList() => _dropdown.GetAllLevelOneId();

        [HttpGet("GetAllDataList")]
        public List<AccILedger> GetAllDataList(int LevelOneAutoID) => _dropdown.GetAllDataList(LevelOneAutoID);

        [HttpGet("GetAllSubAccList")]
        public List<AccLevelThree> GetAllSubAccList(int id) => _dropdown.GetAllSubAccList(id);

        [HttpGet("Ledger_LevelTwoReport")]
        public List<Ledger_Rpt_LevelTwo> GetAllLedgerRptLevelTwoData() => _dropdown.GetAllLedgerRptLevelTwoData();

        [HttpGet("Ledger_LevelThreeReport")]
        public List<Ledger_LevelThreeReport> Get_LedgerlLevelThreeData() => _dropdown.Get_LedgerlLevelThreeData();

        [HttpGet("GetAllChartOfAccLevelOneIdList")]
        public List<ChartOfAccLevelOneIdList> GetAllChartOfAccLevelOneIdList() => _dropdown.GetAllChartOfAccLevelOneId();

        [HttpGet("GetAllLevelTwoIdList")]
        public List<LevelTwoIdList> GetAllLevelTwoIdList() => _dropdown.GetAllLevelTwoId();

        [HttpGet("GetAllLevelThreeIdDescrp")]
        //public List<SubLedgerDdl> GetAllLevelThreeIdDescrp() => _dropdown.GetAllLevelThreeIdDescrp();

        [HttpGet("GetAllAccountCodeLoad")]
        public List<AccCodeLoad> GetAllAccountCodeLoad() => _dropdown.GetAllAccCodeLoad();

        [HttpGet("GetAllAccountNameLoad")]
        public List<AccNameLoad> GetAllAccountNameLoad() => _dropdown.GetAllAccNameLoad();

        [HttpGet("GetAllCategory")]
        public List<CategoryForDropDown> GetAllCategory() => _dropdown.GetAllCategory();

        [HttpGet("GetAllBrands")]
        public List<BrandsForDropDown> GetAllBrands() => _dropdown.GetAllBrands();

        [HttpGet("GetAllItemNameListWithId")]
        public List<ItemsForDropDown> GetAllItemNameListWithId() => _dropdown.GetAllItemName();

        [HttpGet("GetAllItemWithDescription")]
        public List<ItemsForDropDown> GetAllItemWithDescription() => _dropdown.GetAllItemWithDesc();

        [HttpGet("GetSizeColorByStyleList")]
        //public List<SizeColorDropdown> GetSizeColorByStyleList(int ItemId) => _dropdown.GetSizeColor(ItemId);

        [HttpGet("GetVariantListByItemId")]
        //public List<VariantListForDropdown> GetVariantListByItemId(int id, int? whId) => _dropdown.GetVariantByItemId(id, whId);

        [HttpGet("GetAllSupplier")]
        //public List<SupplieresForDropdown> GetAllSupplier() => _dropdown.GetAllSupplier();

        [HttpGet("GetItemsByWarehouse")]
        public List<ItemNameList> GetItemsByWarehouse(int whId) => _dropdown.GetItemsByWarehouse(whId);

        [HttpGet("LastVoucherNumber")]
        public string? LastVoucherNumber() => _dropdown.LastVoucherNumber();

        [HttpGet("GetAllItemColor")]
        public List<string> GetAllItemColor() => _dropdown.GetAllColor();

        [HttpGet("GetAllItemSize")]
        public List<string> GetAllItemSize() => _dropdown.GetAllSize();

        [HttpGet("GetAllCompanyForAcc")]
        public List<Company> GetAllCompanyForAcc() => _dropdown.GetAllCompanyForAcc();

        [HttpGet("GetAllVoucherNumberWithPrefix")]
        public List<string> GetAllVoucherNumberWithPrefix(string prefix) => _dropdown.GetAllVoucherNumberWithPrefix(prefix);

        [HttpGet("GetAllDepartment")]
        public List<MasterChild> GetAllDepartment() => _dropdown.GetAllDepartment();

        [HttpGet("GetAllPDepartment")]
        public List<MasterParent> GetAllPDepartment() => _dropdown.GetAllPDepartment();

        [HttpGet("GetAllDesignation")]
        public List<MasterChild> GetAllDesignation() => _dropdown.GetAllDesignation();

        // লগইন পেজে logo দেখাতে লাগে, তাই login ছাড়াই খোলা
        [AllowAnonymous]
        [HttpGet("GetLogo")]
        public List<tbl_Company> GetLogo() => _dropdown.Logo();

        [HttpGet("GetAllReceiveNo")]
        public List<ReceiveNoList> GetReceiveNo() => _dropdown.GetAllReceiveNo();

        [AllowAnonymous]
        [HttpGet("GetLogoByCompanyId")]
        public List<tbl_Company> GetLogoByCompanyId(int compId) => _dropdown.GetLogoByCompanyId(compId);

        [HttpGet("GetAllMaterialType")]
        public List<MasterChild> GetAllMaterialType() => _dropdown.GetAllMaterialType();

        [HttpGet("GetAllUnit")]
        public List<MasterChild> GetAllUnit() => _dropdown.GetAllUnit();
    }
}
