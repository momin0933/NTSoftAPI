using Dapper;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class DropDownManager : IDropDown
    {
        readonly NTSoftDbContext _dbContext;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        readonly NTSoftDbContextFactory _factory;
        //public DropDownManager(NTSoftDbContext dbContext, DapperService dapperService)
        //{
        //    _dbContext = dbContext;
        //    _IDapperService = dapperService;
        //    _ICommonService = new CommonService(_dbContext);
        //}
        public DropDownManager(NTSoftDbContextFactory factory, IDapperService dapperService)
        {
            _factory = factory;
            _dbContext = _factory.CreateDbContext();
            _IDapperService = dapperService;
            _ICommonService = new CommonService(_factory);
        }
        public List<MasterChild> GetMasterChildListByAccessKey(string AccessKey)
        {
       
            List<MasterChild> cashFlowList = new List<MasterChild>();
            string Query = "select ChildId,ChildName,Value from Vw_ParentChild where AccessKey = '" + AccessKey + "' AND IsActive = 1";
            cashFlowList = _IDapperService.GetAllByQuery<MasterChild>(Query).ToList();
            return cashFlowList;

        }
        public List<Dropdown> GetTeamListByAccessKey(string AccessKey, int? UnitId)
        {
            if (AccessKey == "UN")
            {
                List<Dropdown> cashFlowList = new List<Dropdown>();
                string Query = "select Id,Name from AcctblMaster where AccessKey = '" + AccessKey + "' AND IsActive = 1";
                cashFlowList = _IDapperService.GetAllByQuery<Dropdown>(Query).ToList();
                return cashFlowList;
            }
            else
            {
                List<Dropdown> cashFlowList = new List<Dropdown>();
                string Query = "select Id,Name from AcctblMasterChild where AccessKey = '" + AccessKey + "' AND IsActive = 1 And MasterId='"+UnitId+"'";
                cashFlowList = _IDapperService.GetAllByQuery<Dropdown>(Query).ToList();
                return cashFlowList;
            }
        
        }
        public List<Company> GetAllCompany()
        {
            try
            {
                var list = new List<Company>();
                list = _IDapperService.GetAllByQuery<Company>("select CompanyId,CompanyName from tbl_Company where shortname = 'WH' and IsActive= 1").ToList();    
                return list;
            }
            catch
            {
                throw;
            }
        }
        public List<ItemNameList> GetAllItemList()
        {
            try
            {
                var itemNameList = _IDapperService.GetAllByQuery<ItemNameList>("select Id,StyleNo from POStblItem where IsActive = 1").ToList();
                return itemNameList;    
            }
            catch 
            {
                throw;
            }
        }


        public List<AccILedger> GetAllDataList(int LevelOneAutoID)
        {
            List<AccILedger> getAllDataList = new List<AccILedger>();

            // Assuming LevelOneIdList has a property named LevelOneCode
            getAllDataList = _IDapperService.GetAllByQuery<AccILedger>($"select * from tbl_accleveltwo where LevelOneAutoID={LevelOneAutoID};").ToList();


            return getAllDataList;
        }



        

        public List<AccLevelThree> GetAllSubAccList(int LevelTwoAutoId)
        {
            List<AccLevelThree> getAllSubAccList = new List<AccLevelThree>();
            string query = "select * from AcctblLevelThree where LevelTwoAutoId="+ LevelTwoAutoId + "";
            // string query = "SELECT * FROM AcctblLevelThree WHERE LevelThreeCode LIKE @LevelThreeCode";

            getAllSubAccList = _IDapperService.GetAllByQuery<AccLevelThree>(query).ToList();


            return getAllSubAccList;
        }

 


        //subAccount-End



        #region  
        //public List<LevelOneIdList> GetAllLevelOneId()
        //{
        //    List<LevelOneIdList> levelOneIdList = new List<LevelOneIdList>();

        //    // Assuming LevelOneIdList has a property named LevelOneCode
        //    levelOneIdList = _IDapperService.GetAllByQuery<LevelOneIdList>("SELECT DISTINCT Id, LevelOneCode + '-' + LevelOneDescription AS LevelOneCode FROM tbl_AccLevelOne;").ToList();

        //    return levelOneIdList;
        //}

        #region 
        //ledgerReportDropdown
        //public List<LevelOneDescription> GetAllLevelOneDescription()
        //{
        //    List<LevelOneDescription> levelOneDescription = new List<LevelOneDescription>();

        //    // Assuming LevelOneIdList has a property named LevelOneCode
        //    levelOneDescription = _IDapperService.GetAllByQuery<LevelOneDescription>("SELECT DISTINCT Id, LevelOneDescription as LevelOneCode FROM tbl_AccLevelOne;").ToList();
          
        //    return levelOneDescription;
        //}


        public List<Ledger_Rpt_LevelTwo> GetAllLedgerRptLevelTwoData()
        {

            List<Ledger_Rpt_LevelTwo> levelTwoIdList = new List<Ledger_Rpt_LevelTwo>();
            levelTwoIdList = _IDapperService.GetAllByQuery<Ledger_Rpt_LevelTwo>("select DISTINCT Id, LevelOneAutoID,   LevelTwoDescription   from tbl_AccLevelTwo;").ToList();
            return levelTwoIdList;

        }       


        public List<Ledger_LevelThreeReport> Get_LedgerlLevelThreeData()
        {
            List<Ledger_LevelThreeReport> LevelThreeReportlist = new List<Ledger_LevelThreeReport>();
            LevelThreeReportlist = _IDapperService.GetAllByQuery<Ledger_LevelThreeReport>("select DISTINCT  Id, LevelTwoAutoID, LevelThreeDescription As LevelThreeCode from AcctblLevelThree").ToList();

            return LevelThreeReportlist;
        }

      

        #endregion    

        public List<ChartOfAccLevelOneIdList> GetAllChartOfAccLevelOneId()
        {
            List<ChartOfAccLevelOneIdList> chartOfAcclevelOneIdList = new List<ChartOfAccLevelOneIdList>();

            // Assuming LevelOneIdList has a property named LevelOneCode
            chartOfAcclevelOneIdList = _IDapperService.GetAllByQuery<ChartOfAccLevelOneIdList>("SELECT DISTINCT Id, LevelOneCode + '-' + LevelOneDescription AS LevelOneCode FROM tbl_AccLevelOne;").ToList();

            return chartOfAcclevelOneIdList;
        }




        //public List<LevelTwoIdList> GetAllLevelTwoId()
        //{

        //    List<LevelTwoIdList> levelTwoIdList = new List<LevelTwoIdList>();
        //    levelTwoIdList = _IDapperService.GetAllByQuery<LevelTwoIdList>("select DISTINCT  Id, Code + '-' + LevelTwoDescription As LevelTwoCode from AcctblILedger;").ToList();
        //    return levelTwoIdList;

        //}

        //public List<SubLedgerDdl> GetAllLevelThreeIdDescrp()
        //{

        //    List<SubLedgerDdl> SubLedgerDdlList = new List<SubLedgerDdl>();
        //    SubLedgerDdlList = _IDapperService.GetAllByQuery<SubLedgerDdl>("SELECT DISTINCT Id, LevelThreeCode + '-' + LevelThreeDescription AS LevelThreeDescription FROM AcctblLevelThree;").ToList();
        //    return SubLedgerDdlList;

        //}




       



        public List<AccCodeLoad> GetAllAccCodeLoad()
        {
            List<AccCodeLoad> accCodeLoads = new List<AccCodeLoad>();

            accCodeLoads =_IDapperService.GetAllByQuery<AccCodeLoad>("select DISTINCT AccCode from Vw_ChartOfAcc").ToList() ;

            return accCodeLoads;
        }


 

        public List<AccNameLoad> GetAllAccNameLoad()
        {
            List<AccNameLoad> accNameLoads = new List<AccNameLoad>();
            accNameLoads = _IDapperService.GetAllByQuery<AccNameLoad>("SELECT DISTINCT AccName FROM Vw_ChartOfAcc").ToList();

            return accNameLoads;
        }


   
        #endregion  

        public List<BrandsForDropDown> GetAllBrands()
        {
            List<BrandsForDropDown> brandList = new List<BrandsForDropDown>();
            brandList = _IDapperService.GetAllByQuery<BrandsForDropDown>("Select Name from POStblBrand where IsActive= 1").ToList();
            return brandList;
        }
        public List<CategoryForDropDown> GetAllCategory()
        {
            List<CategoryForDropDown> categoryList = new List<CategoryForDropDown>();
            categoryList = _IDapperService.GetAllByQuery<CategoryForDropDown>("Select Name from POStblCategory where IsActive= 1").ToList();
            return categoryList;
        }
        public List<ItemsForDropDown> GetAllItemName()
        {
            List<ItemsForDropDown> itemNameList = new List<ItemsForDropDown>();
            itemNameList = _IDapperService.GetAllByQuery<ItemsForDropDown>("select Id,StyleNo from postblitem where isActive = 1").OrderByDescending(x=>x.id).ToList();
            return itemNameList;
        }
        //public List<SupplieresForDropdown> GetAllSupplier()
        //{
        //    List<SupplieresForDropdown> supplierList = new List<SupplieresForDropdown>();
        //    //supplierList = _IDapperService.GetAllByQuery<SupplieresForDropdown>("select id,Name from POStblSupplier where IsActive = 1;").ToList();
        //    //supplierList = _IDapperService.GetAllByQuery<SupplieresForDropdown>("select s.id,s.Name,al.Id As LedgerId from POStblSupplier AS s left Join AcctblILedger al On al.Code=s.SuppCode where s.IsActive = 1;").ToList();
        //    supplierList = _IDapperService.GetAllByQuery<SupplieresForDropdown>("SELECT s.id,s.Name, al.Id AS LedgerId FROM POStblSupplier AS s LEFT JOIN AcctblILedger al ON al.Code = CAST(s.SuppCode AS VARCHAR(100)) WHERE s.IsActive = 1;").ToList();
        //    return supplierList;
        //}
        public List<SupplieresForDropdown> GetAllSupplier()
        {
            try
            {
                string SP = "[SP_POSGetLedgerByCode]";

                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 2);

                List<SupplieresForDropdown> supplierList =
                    _IDapperService.GetAllBySP<SupplieresForDropdown>(SP, p).ToList();

                return supplierList;
            }
            catch
            {
                throw;
            }
        }


        public List<ItemNameList> GetItemsByWarehouse(int whId)
        {
            try
            {
                string SP = "[SP_GetItemsByWarehouse]";
                DynamicParameters p = new DynamicParameters();
                p.Add("@WHId", whId);
                List<ItemNameList> ItemList = _IDapperService.GetAllBySP<ItemNameList>(SP, p).ToList();
                return ItemList;
            }
            catch
            {
                throw;
            }
      
        }

        public string LastVoucherNumber()
        {
            try
            {
                var voucherNumber = _IDapperService.GetAllByQuery<string>("SELECT VoucherNumber FROM AcctblVoucher ORDER BY VoucherNumber DESC");
                return voucherNumber.FirstOrDefault(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null; 
            }
        }

        public List<string> GetAllColor()
        {
            List<string> colorList= new List<string>();
            colorList = _IDapperService.GetAllByQuery<string>("select distinct color from  POStblItemVariant where IsActive= 1").ToList();
            return colorList;
        } 
        public List<string> GetAllSize()
        {
            List<string> sizeList = new List<string>();
            sizeList = _IDapperService.GetAllByQuery<string>("select distinct Size from  POStblItemVariant where IsActive= 1").ToList();
            return sizeList;
        }
        public List<Company> GetAllCompanyForAcc()
        {
            try
            {
                var list = new List<Company>();
                list = _IDapperService.GetAllByQuery<Company>("select CompanyId,CompanyName from tbl_Company where IsActive =1").ToList();
                return list;
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetAllVoucherNumberWithPrefix(string typeString)
        {
            try
            {
              
                string query = "  SELECT VoucherNumber FROM AcctblVoucher WHERE VoucherNumber = (SELECT MAX(VoucherNumber) FROM AcctblVoucher WHERE VoucherNumber LIKE '"+ typeString + "%');";
                List<string> list = _IDapperService.GetAllByQuery<string>(query).ToList();
                return list;
            }
            catch
            {
                throw;
            }

        }

        public List<MasterChild> GetAllDepartment()
        {
            var list = new List<MasterChild>();
            list = _IDapperService.GetAllByQuery<MasterChild>("select ChildId,ChildName from tblMaster_Child  WHERE AccessKey ='Dept' ORDER BY SortBy ").ToList();
            return list;
        }

        public List<MasterParent> GetAllPDepartment()
        {
            var list = new List<MasterParent>();
            list = _IDapperService.GetAllByQuery<MasterParent>("Select ParentId,ParentName from tblMaster_Parent  WHERE AccessKey ='Dept' ").ToList();
            return list;
        }

        public List<MasterChild> GetAllDesignation()
        {
            var list = new List<MasterChild>();
            list= _IDapperService.GetAllByQuery<MasterChild>("select ChildId,ChildName from tblMaster_Child  WHERE AccessKey ='Deg'").ToList();
            return list;
        }

        public List<tbl_Company> Logo()
        {
            var list = new List<tbl_Company>();
            list = _IDapperService.GetAllByQuery<tbl_Company>("select CompanyId,  LogoPath,CompanyAddress,ShortName,ContactNo from tbl_Company where  IsActive= 1").ToList();
            return list;
        }

        public List<VariantListForDropdown> GetVariantByItemId(int id, int? whId)
        {
            if(whId != null)
            {
                string SP = "GetVariantListWithStock";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker",1);

                p.Add("@WHId", whId);
                p.Add("@Id", id);
                List<VariantListForDropdown> ItemList = _IDapperService.GetAllBySP<VariantListForDropdown>(SP, p).ToList();
                return ItemList;
            }
            else
            {
                string SP = "GetVariantListWithStock";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 2);
                p.Add("@Id", id);
                List<VariantListForDropdown> ItemList = _IDapperService.GetAllBySP<VariantListForDropdown>(SP, p).ToList();
                return ItemList;
            }
           
        }

        //public List<ReceiveNoList> GetAllReceiveNo()
        //{
        //    var list = new List<ReceiveNoList>();
        //    list = _IDapperService.GetAllByQuery<ReceiveNoList>("select Id, ReceiveNo from POStblItemRcv where ReceiveNo Not Like '%TN%'").ToList();
        //    return list;
        //}

        public List<tbl_Company> GetLogoByCompanyId(int compId)
        {
            var list = new List<tbl_Company>();
            list = _IDapperService.GetAllByQuery<tbl_Company>("select CompanyId, CompanyName,LogoPath,LabelPath,ReceiptPath,CompanyAddress,ShortName,ContactNo,CompanyType from tbl_Company where  IsActive= 1 AND CompanyId = '"+compId+"'").ToList();
            return list;
        }
  
        // Style + Description Dropdown
        public List<ItemsForDropDown> GetAllItemWithDesc()
        {
            try
            {
                string SP = "[SP_ItemTransactionReport]";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 2);

                return _IDapperService
                    .GetAllBySP<ItemsForDropDown>(SP, p)
                    .OrderByDescending(x => x.id)
                    .ToList();
            }
            catch
            {
                throw;
            }
        }

        public List<SizeColorDropdown> GetSizeColor(int ItemId)
        {
            try
            {
                string SP = "[SP_ItemTransactionReport]";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 3);
                p.Add("@ItemId", ItemId);

                return _IDapperService.GetAllBySP<SizeColorDropdown>(SP, p).ToList();
            }
            catch
            {
                throw;
            }
        }

        public List<MasterChild> GetAllMaterialType()
        {
            var list = new List<MasterChild>();
            list = _IDapperService.GetAllByQuery<MasterChild>("select ChildId,ChildName from tblMaster_Child  WHERE AccessKey = 'ProductType' And Isactive =1").ToList();
            return list;
        }

        public List<MasterChild> GetAllUnit()
        {
            var list = new List<MasterChild>();
            list = _IDapperService.GetAllByQuery<MasterChild>("select ChildId,ChildName from tblMaster_Child  WHERE AccessKey ='UnitType' and IsActive =1").ToList();
            return list;
        }

        //public List<CustomerDropdown> GetAllCustomer()
        //{
        //    try
        //    {
        //        var list = new List<CustomerDropdown>();
        //        list= _IDapperService.GetAllByQuery<CustomerDropdown>(
        //            "SELECT Id, Name, PhoneNo FROM POStblCustomer"
        //        ).ToList();
        //        return list;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        public List<CustomerDropdown> GetAllCustomer()
        {
            try
            {
                string SP = "[SP_POSGetLedgerByCode]";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 4);

                return _IDapperService
                        .GetAllBySP<CustomerDropdown>(SP, p)
                        .ToList();
            }
            catch
            {
                throw;
            }
        }
        public List<InvoiceDropdown> GetAllInvoice()
        {
            try
            {
                var list = new List<InvoiceDropdown>();
                list = _IDapperService.GetAllByQuery<InvoiceDropdown>(
                    "SELECT i.Id,i.InvoiceNo,i.CustomerNumber,c.Name AS CustomerName FROM POStblInvoice as i LEFT JOIN POStblCustomer as c ON c.PhoneNo = i.CustomerNumber WHERE ISNULL(i.PaidAmount,0) < i.Total"
                ).ToList();
                return list;
            }
            catch
            {
                throw;
            }
        }
    }
}
