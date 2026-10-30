
using InventoryAccountsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace InventoryAccountsAPI.BusinessLayer.Interface
{
    public interface IDropDown 
    {
        public List<Company> GetAllCompany();
        public List<CustomerDropdown> GetAllCustomer();
        public List<InvoiceDropdown> GetAllInvoice();
        public List<ItemNameList> GetAllItemList();
        //public List<LevelOneIdList> GetAllLevelOneId();
        //public List<LevelOneDescription> GetAllLevelOneDescription(); 
        public List<Ledger_Rpt_LevelTwo> GetAllLedgerRptLevelTwoData();      
        public List<Ledger_LevelThreeReport> Get_LedgerlLevelThreeData();
      //  public List<LevelThreeDescrip> GetAllLevelThreeDescription(); 
        public List<ChartOfAccLevelOneIdList> GetAllChartOfAccLevelOneId();
        //public List<LevelTwoIdList> GetAllLevelTwoId(); 
        //public List<SubLedgerDdl> GetAllLevelThreeIdDescrp();
        public List<AccILedger> GetAllDataList(int LevelOneAutoID);

        //public List<AccLevelThree> GetAllSubAccList(string LevelThreeCode);
        public List<MasterChild> GetMasterChildListByAccessKey(string AccessKey);  
        public List<Dropdown> GetTeamListByAccessKey(string AccessKey, int? UnitId);
        public List<AccCodeLoad> GetAllAccCodeLoad();
        public List<AccNameLoad> GetAllAccNameLoad();
        public List<BrandsForDropDown> GetAllBrands();
        public List<CategoryForDropDown> GetAllCategory();
        public List<ItemsForDropDown> GetAllItemName();
        public List<ItemsForDropDown> GetAllItemWithDesc();
        public List<SizeColorDropdown> GetSizeColor(int ItemId);


        public List<VariantListForDropdown> GetVariantByItemId(int id, int? whId);

        public List<SupplieresForDropdown> GetAllSupplier();
        public List<ItemNameList> GetItemsByWarehouse(int whId);
        public List<AccLevelThree> GetAllSubAccList(int LevelTwoAutoId);

        public string LastVoucherNumber();
        public List<string> GetAllColor();
        public List<string> GetAllSize();
        public List<Company> GetAllCompanyForAcc();
        public List<string> GetAllVoucherNumberWithPrefix(string typeString);
        public List<MasterChild> GetAllDepartment();
        public List<MasterParent> GetAllPDepartment();
        public List<MasterChild> GetAllDesignation();

        public List<tbl_Company> Logo();
        //public List<ReceiveNoList> GetAllReceiveNo();

        public List<tbl_Company> GetLogoByCompanyId( int compId);
        public List<MasterChild> GetAllMaterialType();
        public List<MasterChild> GetAllUnit();


    }
}
