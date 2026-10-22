using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using BMSAPI.Models.Common;
using Dapper;

namespace BMSAPI.BusinessLayer.Manager
{
    public class DropDownManager : IDropDown
    {
        private readonly IDapperService _dapper;

        public DropDownManager(IDapperService dapper)
        {
            _dapper = dapper;
        }

        private List<T> Query<T>(string sql, object? param = null) where T : class
            => _dapper.GetAllByQuery<T>(sql, param).ToList();

        private List<T> Sp<T>(string sp, DynamicParameters p) where T : class
            => _dapper.GetAllBySP<T>(sp, p).ToList();

        // ---------------- Master / child ----------------

        public List<MasterChild> GetMasterChildListByAccessKey(string AccessKey)
            => Query<MasterChild>(
                "SELECT ChildId, ChildName, Value FROM Vw_ParentChild WHERE AccessKey = @AccessKey AND IsActive = 1",
                new { AccessKey });

        public List<Dropdown> GetTeamListByAccessKey(string AccessKey, int? UnitId)
        {
            if (AccessKey == "UN")
                return Query<Dropdown>(
                    "SELECT Id, Name FROM AcctblMaster WHERE AccessKey = @AccessKey AND IsActive = 1",
                    new { AccessKey });

            return Query<Dropdown>(
                "SELECT Id, Name FROM AcctblMasterChild WHERE AccessKey = @AccessKey AND IsActive = 1 AND MasterId = @UnitId",
                new { AccessKey, UnitId });
        }

        public List<MasterChild> GetAllDepartment()
            => Query<MasterChild>("SELECT ChildId, ChildName FROM tblMaster_Child WHERE AccessKey = 'Dept' ORDER BY SortBy");

        public List<MasterParent> GetAllPDepartment()
            => Query<MasterParent>("SELECT ParentId, ParentName FROM tblMaster_Parent WHERE AccessKey = 'Dept'");

        public List<MasterChild> GetAllDesignation()
            => Query<MasterChild>("SELECT ChildId, ChildName FROM tblMaster_Child WHERE AccessKey = 'Deg'");

        public List<MasterChild> GetAllMaterialType()
            => Query<MasterChild>("SELECT ChildId, ChildName FROM tblMaster_Child WHERE AccessKey = 'ProductType' AND IsActive = 1");

        public List<MasterChild> GetAllUnit()
            => Query<MasterChild>("SELECT ChildId, ChildName FROM tblMaster_Child WHERE AccessKey = 'UnitType' AND IsActive = 1");

        // ---------------- Company ----------------

        public List<Company> GetAllCompany()
            => Query<Company>("SELECT CompanyId, CompanyName FROM tbl_Company WHERE ShortName = 'WH' AND IsActive = 1");

        public List<Company> GetAllCompanyForAcc()
            => Query<Company>("SELECT CompanyId, CompanyName FROM tbl_Company WHERE IsActive = 1");

        public List<tbl_Company> Logo()
            => Query<tbl_Company>("SELECT CompanyId, LogoPath, CompanyAddress, ShortName, ContactNo FROM tbl_Company WHERE IsActive = 1");

        public List<tbl_Company> GetLogoByCompanyId(int compId)
            => Query<tbl_Company>(
                @"SELECT CompanyId, CompanyName, LogoPath, LabelPath, ReceiptPath, CompanyAddress, ShortName, ContactNo, CompanyType
                  FROM tbl_Company WHERE IsActive = 1 AND CompanyId = @compId",
                new { compId });

        // ---------------- Chart of accounts ----------------

        public List<AccILedger> GetAllDataList(int LevelOneAutoID)
            => Query<AccILedger>("SELECT * FROM tbl_AccLevelTwo WHERE LevelOneAutoID = @LevelOneAutoID", new { LevelOneAutoID });

        public List<AccLevelThree> GetAllSubAccList(int LevelTwoAutoId)
            => Query<AccLevelThree>("SELECT * FROM AcctblLevelThree WHERE LevelTwoAutoId = @LevelTwoAutoId", new { LevelTwoAutoId });

        public List<LevelOneIdList> GetAllLevelOneId()
            => Query<LevelOneIdList>("SELECT DISTINCT Id, LevelOneCode + '-' + LevelOneDescription AS LevelOneCode FROM tbl_AccLevelOne");

        public List<LevelOneDescription> GetAllLevelOneDescription()
            => Query<LevelOneDescription>("SELECT DISTINCT Id, LevelOneDescription AS LevelOneCode FROM tbl_AccLevelOne");

        public List<Ledger_Rpt_LevelTwo> GetAllLedgerRptLevelTwoData()
            => Query<Ledger_Rpt_LevelTwo>("SELECT DISTINCT Id, LevelOneAutoID, LevelTwoDescription FROM tbl_AccLevelTwo");

        public List<Ledger_LevelThreeReport> Get_LedgerlLevelThreeData()
            => Query<Ledger_LevelThreeReport>("SELECT DISTINCT Id, LevelTwoAutoID, LevelThreeDescription AS LevelThreeCode FROM AcctblLevelThree");

        public List<ChartOfAccLevelOneIdList> GetAllChartOfAccLevelOneId()
            => Query<ChartOfAccLevelOneIdList>("SELECT DISTINCT Id, LevelOneCode + '-' + LevelOneDescription AS LevelOneCode FROM tbl_AccLevelOne");

        public List<LevelTwoIdList> GetAllLevelTwoId()
            => Query<LevelTwoIdList>("SELECT DISTINCT Id, Code + '-' + LevelTwoDescription AS LevelTwoCode FROM AcctblILedger");

        //public List<SubLedgerDdl> GetAllLevelThreeIdDescrp()
        //    => Query<SubLedgerDdl>("SELECT DISTINCT Id, LevelThreeCode + '-' + LevelThreeDescription AS LevelThreeDescription FROM AcctblLevelThree");

        public List<AccCodeLoad> GetAllAccCodeLoad()
            => Query<AccCodeLoad>("SELECT DISTINCT AccCode FROM Vw_ChartOfAcc");

        public List<AccNameLoad> GetAllAccNameLoad()
            => Query<AccNameLoad>("SELECT DISTINCT AccName FROM Vw_ChartOfAcc");

        // ---------------- Voucher numbers ----------------

        public string? LastVoucherNumber()
            => _dapper.GetSingle<string>("SELECT TOP 1 VoucherNumber FROM AcctblVoucher ORDER BY Id DESC", new { });

        public List<string> GetAllVoucherNumberWithPrefix(string typeString)
        {
            if (string.IsNullOrWhiteSpace(typeString))
                return new List<string>();

            // [ % _ LIKE-এ বিশেষ অর্থ বহন করে, তাই escape করা হলো
            var pattern = typeString.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]") + "%";

            // "JV#9" আর "JV#10"-এর মধ্যে সংখ্যা হিসেবে বড়টা বেছে নেওয়া হয়
            return Query<string>(
                @"SELECT TOP 1 VoucherNumber
                  FROM AcctblVoucher
                  WHERE VoucherNumber LIKE @pattern
                  ORDER BY TRY_CAST(SUBSTRING(VoucherNumber, CHARINDEX('#', VoucherNumber) + 1, 50) AS INT) DESC,
                           VoucherNumber DESC",
                new { pattern });
        }

        // ---------------- POS: customer / supplier / invoice ----------------

        public List<CustomerDropdown> GetAllCustomer()
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 4);
            return Sp<CustomerDropdown>("SP_POSGetLedgerByCode", p);
        }

        //public List<SupplieresForDropdown> GetAllSupplier()
        //{
        //    var p = new DynamicParameters();
        //    p.Add("@QueryChecker", 2);
        //    return Sp<SupplieresForDropdown>("SP_POSGetLedgerByCode", p);
        //}

        public List<InvoiceDropdown> GetAllInvoice()
            => Query<InvoiceDropdown>(
                @"SELECT i.Id, i.InvoiceNo, i.CustomerNumber, c.Name AS CustomerName
                  FROM POStblInvoice AS i
                  LEFT JOIN POStblCustomer AS c ON c.PhoneNo = i.CustomerNumber
                  WHERE ISNULL(i.PaidAmount, 0) < i.Total");

        public List<ReceiveNoList> GetAllReceiveNo()
            => Query<ReceiveNoList>("SELECT Id, ReceiveNo FROM POStblItemRcv WHERE ReceiveNo NOT LIKE '%TN%'");

        // ---------------- POS: items ----------------

        public List<ItemNameList> GetAllItemList()
            => Query<ItemNameList>("SELECT Id, StyleNo FROM POStblItem WHERE IsActive = 1");

        public List<ItemsForDropDown> GetAllItemName()
            => Query<ItemsForDropDown>("SELECT Id, StyleNo FROM POStblItem WHERE IsActive = 1 ORDER BY Id DESC");

        public List<ItemsForDropDown> GetAllItemWithDesc()
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 2);
            return Sp<ItemsForDropDown>("SP_ItemTransactionReport", p).OrderByDescending(x => x.id).ToList();
        }

        //public List<SizeColorDropdown> GetSizeColor(int ItemId)
        //{
        //    var p = new DynamicParameters();
        //    p.Add("@QueryChecker", 3);
        //    p.Add("@ItemId", ItemId);
        //    return Sp<SizeColorDropdown>("SP_ItemTransactionReport", p);
        //}

        public List<ItemNameList> GetItemsByWarehouse(int whId)
        {
            var p = new DynamicParameters();
            p.Add("@WHId", whId);
            return Sp<ItemNameList>("SP_GetItemsByWarehouse", p);
        }

        //public List<VariantListForDropdown> GetVariantByItemId(int id, int? whId)
        //{
        //    var p = new DynamicParameters();
        //    p.Add("@QueryChecker", whId != null ? 1 : 2);
        //    p.Add("@Id", id);
        //    if (whId != null)
        //        p.Add("@WHId", whId);
        //    return Sp<VariantListForDropdown>("GetVariantListWithStock", p);
        //}

        public List<BrandsForDropDown> GetAllBrands()
            => Query<BrandsForDropDown>("SELECT Name FROM POStblBrand WHERE IsActive = 1");

        public List<CategoryForDropDown> GetAllCategory()
            => Query<CategoryForDropDown>("SELECT Name FROM POStblCategory WHERE IsActive = 1");

        public List<string> GetAllColor()
            => Query<string>("SELECT DISTINCT Color FROM POStblItemVariant WHERE IsActive = 1");

        public List<string> GetAllSize()
            => Query<string>("SELECT DISTINCT Size FROM POStblItemVariant WHERE IsActive = 1");
    }
}