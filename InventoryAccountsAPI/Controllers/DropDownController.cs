
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InventoryAccountsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DropDownController : ControllerBase
    {
        private readonly IDropDown _dropdown;

        public DropDownController(IDropDown drpDown)
        {
            _dropdown = drpDown;
        }

        [Route("GetMasterChildList")]  //// Master Child List
        [HttpGet]
        public IEnumerable<MasterChild> GetMasterChildList(string AccessKey)
        {
    
                List<MasterChild> cList = new List<MasterChild>();
                cList = _dropdown.GetMasterChildListByAccessKey(AccessKey);
                return cList;
                
        }
        [Route("GetUnitTeamList")]  //// Master Child List
        [HttpGet]
        public IEnumerable<Dropdown> GetUnitAndTeamList(string AccessKey, int? UnitId)
        {

            List<Dropdown> cList = new List<Dropdown>();
            cList = _dropdown.GetTeamListByAccessKey(AccessKey, UnitId);
            return cList;

        }
        [Route("GetCompanyList")]  //// company= Warehouse
        [HttpGet]
        public List<Company> GetCompanyList()
        {
            List<Company> cList = new List<Company>();
            cList = _dropdown.GetAllCompany().ToList();
            return cList;
        }

        [Route("GetCustomerList")]
        [HttpGet]
        public List<CustomerDropdown> GetCustomerList()
        {
            List<CustomerDropdown> customerList = new List<CustomerDropdown>();
            customerList= _dropdown.GetAllCustomer().ToList();
            return customerList;
        }

        [Route("GetInvoiceList")]
        [HttpGet]
        public List<InvoiceDropdown> GetInvoiceList()
        {
            List<InvoiceDropdown> invoiceList = new List<InvoiceDropdown>();
            invoiceList = _dropdown.GetAllInvoice().ToList();
            return invoiceList;
        }

        [Route("GetItemNameList")]
        [HttpGet]
        public List<ItemNameList> GetItemNameList()
        {
            List<ItemNameList> itemNameLists = new List<ItemNameList>();
            itemNameLists = _dropdown.GetAllItemList().ToList();
            return itemNameLists;
        }

        //[Route("GetAllLevelOneIdList")]
        //[HttpGet]
        //public List<LevelOneIdList> GetAllLevelOneIdList()
        //{
        //    List<LevelOneIdList> list = new List<LevelOneIdList>();
        //    list = _dropdown.GetAllLevelOneId().ToList();
        //    return list;

        //}

        [Route("GetAllDataList")]
        [HttpGet]
        public List<AccILedger> GetAllDataList(int LevelOneAutoID)
        {
            List<AccILedger> list = new List<AccILedger>();
            list = _dropdown.GetAllDataList(LevelOneAutoID).ToList();
            return list;

        }

        [Route("GetAllSubAccList")]
        [HttpGet]
        public List<AccLevelThree> GetAllSubAccList(int id)
        {
            List<AccLevelThree> list = new List<AccLevelThree>();
            list = _dropdown.GetAllSubAccList(id).ToList();
            return list;

        }

        [Route("Ledger_LevelTwoReport")]
        [HttpGet]
        public List<Ledger_Rpt_LevelTwo> GetAllLedgerRptLevelTwoData()
        {
            List<Ledger_Rpt_LevelTwo> list = new List<Ledger_Rpt_LevelTwo>();
            list = _dropdown.GetAllLedgerRptLevelTwoData().ToList();
            return list;

        }

        [Route("Ledger_LevelThreeReport")]
        [HttpGet]
        public List<Ledger_LevelThreeReport> Get_LedgerlLevelThreeData()
        {
            List<Ledger_LevelThreeReport> list = new List<Ledger_LevelThreeReport>();
            list = _dropdown.Get_LedgerlLevelThreeData().ToList();
            return list;

        }

        [Route("GetAllChartOfAccLevelOneIdList")]
        [HttpGet]
        public List<ChartOfAccLevelOneIdList> GetAllChartOfAccLevelOneIdList()
        {
            List<ChartOfAccLevelOneIdList> list = new List<ChartOfAccLevelOneIdList>();
            list = _dropdown.GetAllChartOfAccLevelOneId().ToList();
            return list;

        }

        //[Route("GetAllLevelTwoIdList")]
        //[HttpGet]
        //public List<LevelTwoIdList> GetAllLevelTwoIdList()
        //{
        //    List<LevelTwoIdList> list = new List<LevelTwoIdList>();
        //    list = _dropdown.GetAllLevelTwoId().ToList();
        //    return list;

        //}

        //[Route("GetAllLevelThreeIdDescrp")]
        //[HttpGet]
        //public List<SubLedgerDdl> GetAllLevelThreeIdDescrp()
        //{
        //    List<SubLedgerDdl> list = new List<SubLedgerDdl>();
        //    list = _dropdown.GetAllLevelThreeIdDescrp().ToList();
        //    return list;

        //}

        [Route("GetAllAccountCodeLoad")]
        [HttpGet]
        public List<AccCodeLoad> GetAllAccountCodeLoad()
        {
            List<AccCodeLoad> list = new List<AccCodeLoad>();
            list = _dropdown.GetAllAccCodeLoad().ToList();
            return list;

        }


        [Route("GetAllAccountNameLoad")]
        [HttpGet]
        public List<AccNameLoad> GetAllAccountNameLoad()
        {
            List<AccNameLoad> list = new List<AccNameLoad>();
            list = _dropdown.GetAllAccNameLoad().ToList();
            return list;

        }
        [Route("GetAllCategory")]
        [HttpGet]
        public List<CategoryForDropDown> GetAllCategory()
        {
            List<CategoryForDropDown> categoryList = new List<CategoryForDropDown>();
            categoryList = _dropdown.GetAllCategory().ToList();
            return categoryList;
        }
        [Route("GetAllBrands")]
        [HttpGet]
        public List<BrandsForDropDown> GetAllBrands()
        {
            List<BrandsForDropDown> brandList = new List<BrandsForDropDown>();
            brandList = _dropdown.GetAllBrands().ToList();
            return brandList;
        }
        [Route("GetAllItemNameListWithId")]
        [HttpGet]
        public List<ItemsForDropDown> GetAllItemNameListWithId()
        {
            List<ItemsForDropDown> itemNameList = new List<ItemsForDropDown>();
            itemNameList = _dropdown.GetAllItemName().ToList();
            return itemNameList;
        }
   
        // Get all styles for dropdown
        [HttpGet("GetAllItemWithDescription")]
        public List<ItemsForDropDown> GetAllItemWithDescription()
        {
            return _dropdown.GetAllItemWithDesc();
        }

        [HttpGet("GetSizeColorByStyleList")]
        public List<SizeColorDropdown> GetSizeColorByStyleList(int ItemId)
        {
            return _dropdown.GetSizeColor(ItemId);
        }

        [Route("GetVariantListByItemId")]
        [HttpGet]
        public List<VariantListForDropdown> GetVariantListByItemId(int id, int? whId)
        {
            List<VariantListForDropdown> itemNameList = new List<VariantListForDropdown>();
            itemNameList = _dropdown.GetVariantByItemId(id, whId).ToList();
            return itemNameList;
        }
        [Route("GetAllSupplier")]
        [HttpGet]
        public List<SupplieresForDropdown> GetAllSupplier()
        {
            List<SupplieresForDropdown> SupplierList = new List<SupplieresForDropdown>();
            SupplierList = _dropdown.GetAllSupplier().ToList();
            return SupplierList;
        }
        [Route("GetItemsByWarehouse")]
        [HttpGet]
        public List<ItemNameList> GetItemsByWarehouse(int whId)
        {
            List<ItemNameList> SupplierList = new List<ItemNameList>();
            SupplierList = _dropdown.GetItemsByWarehouse(whId).ToList();
            return SupplierList;
        }


        [Route("LastVoucherNumber")]
        [HttpGet]
        public string LastVoucherNumber()
        {

            var SupplierList = _dropdown.LastVoucherNumber();
            return SupplierList;
        }

        [Route("GetAllItemColor")]
        [HttpGet]
        public List<string> GetAllItemColor()
        {
            List<string> colorList = new List<string>();
            colorList = _dropdown.GetAllColor().ToList();
            return colorList;
        }
        [Route("GetAllItemSize")]
        [HttpGet]
        public List<string> GetAllItemSize()
        {
            List<string> sizeList = new List<string>();
            sizeList = _dropdown.GetAllSize().ToList();
            return sizeList;
        }

        [Route("GetAllCompanyForAcc")]
        [HttpGet]
        public List<Company> GetAllCompanyForAcc()
        {
            try
            {
                var list = _dropdown.GetAllCompanyForAcc();
                return list;
            }
            catch
            {
                throw;
            }
       
        }
        [Route("GetAllVoucherNumberWithPrefix")]
        [HttpGet]
        public List<string> GetAllVoucherNumberWithPrefix(string prefix)
        {
            var list = _dropdown.GetAllVoucherNumberWithPrefix(prefix);
            return list;

        }
        [Route("GetAllDepartment")]

        [HttpGet]
        public List<MasterChild> GetAllDepartment()
        {
            try
            {
                List<MasterChild> Itemlist = _dropdown.GetAllDepartment().ToList();
                return Itemlist;
            }
            catch
            {
                throw;
            }
        }

        [Route("GetAllPDepartment")]

        [HttpGet]
        public List<MasterParent> GetAllPDepartment()
        {
            try
            {
                List<MasterParent> Itemlist = _dropdown.GetAllPDepartment().ToList();
                return Itemlist;
            }
            catch
            {
                throw;
            }
        }
        [Route("GetAllDesignation")]
        [HttpGet]
        public List<MasterChild> GetAllDesignation()
        {
            try
            {
                List<MasterChild> Itemlist = _dropdown.GetAllDesignation().ToList();
                return Itemlist;
            }
            catch
            {
                throw;
            }

        }

        [Route("GetLogo")]
        [HttpGet]
        public List<tbl_Company> GetLogo()
        {
            var logoList = _dropdown.Logo();
            return logoList;
        }

        //[Route("GetAllReceiveNo")]
        //[HttpGet]
        //public List<ReceiveNoList> GetReceiveNo()
        //{
        //    var logoList = _dropdown.GetAllReceiveNo();
        //    return logoList;
        //}
        [Route("GetLogoByCompanyId")]
        [HttpGet]
        public List<tbl_Company> GetLogoByCompanyId(int compId)
        {
            var logoList = _dropdown.GetLogoByCompanyId(compId);
            return logoList;
        }
        [Route("GetAllMaterialType")]
        [HttpGet]
        public List<MasterChild> GetAllMaterialType()
        {
            try
            {
                List<MasterChild> Itemlist = _dropdown.GetAllMaterialType().ToList();
                return Itemlist;
            }
            catch
            {
                throw;
            }

        }

        [Route("GetAllUnit")]
        [HttpGet]
        public List<MasterChild> GetAllUnit()
        {
            try
            {
                List<MasterChild> Itemlist = _dropdown.GetAllUnit().ToList();
                return Itemlist;
            }
            catch
            {
                throw;
            }

        }

    }
}
