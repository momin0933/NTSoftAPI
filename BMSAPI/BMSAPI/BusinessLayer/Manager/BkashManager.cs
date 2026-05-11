using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Http; // Add this at the top if not already present

namespace BMSAPI.BusinessLayer.Manager
{
    public class BkashManager: IBkashManager
    {
        private readonly ILogger<BkashManager> _logger;
        readonly NTSoftDbContextFactory _dbContext;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        IUserManager _userData;
        private readonly IHttpContextAccessor _httpContextAccessor; // Add this field

        public BkashManager(
            NTSoftDbContextFactory dbContext,
            IDapperService dapperService,
            ILogger<BkashManager> logger,
            IUserManager userData,
            IHttpContextAccessor httpContextAccessor // Add this parameter
        )
        {            
            _dbContext = dbContext;
            _IDapperService = dapperService;
            _ICommonService = new CommonService(_dbContext);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userData = userData ?? throw new ArgumentNullException(nameof(userData));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor)); // Assign
        }
        
        #region GET Operations (Dapper)
        public BkashBillInfo GetBillMonthWise(string UserName, string Password, string FlatCode, string BillMonth)
        {
            try
            {
                if (UserName != null && Password != null)
                {
                    // 1. Validate user
                    var user = _userData.GetUser(UserName, Password);                    

                    if (user == null)
                    {
                        _logger.LogWarning("User authentication failed for: {UserName}", UserName);
                        return null;
                    }

                    // Use IHttpContextAccessor to access the current HttpContext
                    _httpContextAccessor.HttpContext?.Session.SetString("TenantId", user.TenantId);
                    string procedur = "SP_GetBillForBkash";
                    DynamicParameters p = new DynamicParameters();
                    p.Add("@FlatCode", FlatCode);
                    p.Add("@BillMonth", BillMonth);
                    BkashBillInfo billInfo = _IDapperService.GetAllBySP<BkashBillInfo>(procedur, p).FirstOrDefault();
                    _httpContextAccessor.HttpContext?.Session.Remove("TenantId");                    
                    return billInfo;
                }

                _logger.LogWarning("Invalid input: UserName or Password is null");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bill information");
                throw;
            }
        }

        //public bool SaveBkashPayment(BkashPaymentRequest request)
        //{
        //    try
        //    {              
        //        var user = _userData.GetUser(request.UserName, request.Password);
        //        if (user == null)
        //        {
        //            _logger.LogWarning("User authentication failed for: {UserName}", request.UserName);
        //            return false;
        //        }

        //        _httpContextAccessor.HttpContext?.Session.SetString("TenantId", user.TenantId);
        //        var payment = new VbntblBkashPayments
        //        {               
        //            FlatCode = request.FlatCode,
        //            BillMonth = request.BillMonth,
        //            Amount = request.Amount,
        //            UserMobileNumber = request.UserMobileNumber,
        //            TrxId = request.TrxId,
        //            PayTime = request.PayTime
        //        };

        //        _ICommonService.Add(payment);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error saving bKash payment");
        //        throw;
        //    }
        //}

        public bool SaveBkashPayment(BkashPaymentRequest request)
        {
            try
            {          
                var user = _userData.GetUser(request.UserName, request.Password);

                if (user == null)
                {
                    _logger.LogWarning("User authentication failed for: {UserName}", request.UserName);
                    return false;
                }

                _httpContextAccessor.HttpContext?.Session.SetString("TenantId", user.TenantId);

                string sql = @"
            SELECT TOP 1 
                BillAmount,
                BillNo
            FROM VbntblBill
            WHERE FORMAT([Date], 'MMyyyy') = @BillMonth
              AND FlatCode = @FlatCode
        ";

                var bill = _IDapperService.GetSingle<dynamic>(sql, new
                {
                    BillMonth = request.BillMonth,
                    FlatCode = request.FlatCode
                });

                if (bill == null)
                {
                    _logger.LogWarning("Bill not found for FlatCode: {FlatCode}", request.FlatCode);
                    return false;
                }

                var payment = new VbntblBkashPayments
                {
                    FlatCode = request.FlatCode,
                    BillMonth = request.BillMonth,
                    Amount = request.Amount,
                    UserMobileNumber = request.UserMobileNumber,
                    TrxId = request.TrxId,
                    PayTime = request.PayTime,
                    BillNo = bill.BillNo,
                    EntryBy = "bKash",
                    EntryDate = DateTime.Now,
                    IsActive = true
                };

                _ICommonService.Add(payment).Wait();

                string updateSql = @"
            UPDATE VbntblBill
            SET 
                Collection = @Collection,
                CollectionDate = GETDATE(),
                Status = 'Paid'
            WHERE BillNo = @BillNo";

                _IDapperService.ExecuteAsync(updateSql, new
                {
                    Collection = request.Amount,
                    BillNo = bill.BillNo
                });


                string SP = "Sp_VbnExpense";

                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 13);
                p.Add("@OwnerCode", request.FlatCode);
                p.Add("@VoucherType", "RV");

                var list = _IDapperService
                    .GetAllBySP<VoucherNumber>(SP, p)
                    .FirstOrDefault();

                if (list == null)
                {
                    _logger.LogWarning("Voucher setup not found");
                    return false;
                }

                string newVoucherNumber = "RV#1";

                if (!string.IsNullOrEmpty(list.LastVoucherNumber) &&
                    list.LastVoucherNumber.StartsWith("RV#"))
                {
                    var numberPart = list.LastVoucherNumber.Substring(3);

                    if (int.TryParse(numberPart, out int num))
                    {
                        newVoucherNumber = $"RV#{num + 1}";
                    }
                }

                AccVoucher accVoucher = new AccVoucher
                {
                    VoucherType = "RV",
                    VoucherNumber = newVoucherNumber,
                    VoucherDate = DateTime.Now,
                    PaymentType = "bKash",
                    Narration = "bKash Collection",
                    TotalAmount = request.Amount,
                    VoucherStatus = "Approved",
                    CompanyId = 1,
                    Remarks = bill.BillNo,
                    EntryBy = "bKash",
                    EntryDate = DateTime.Now,
                    IsActive = true
                };

                int voucherId = _ICommonService.Add(accVoucher).Result;

                if (voucherId <= 0)
                {
                    _logger.LogError("AccVoucher insert failed");
                    return false;
                }

 
                VoucherDetails creditEntry = new VoucherDetails
                {
                    AccVoucherId = voucherId,
                    LedgerId = list.LedgerId,
                    TranType = "Cr",
                    Amount = request.Amount,
                    CreditAmount = request.Amount,
                    DebitAmount = 0,
                    ShortDesc = "bKash Collection Credit",
                    PaymentType = "bKash",
                    EntryBy = "bKash",
                    EntryDate = DateTime.Now,
                    IsActive = true
                };

                _ICommonService.Add(creditEntry).Wait();

                VoucherDetails debitEntry = new VoucherDetails
                {
                    AccVoucherId = voucherId,
                    //LedgerId = 2492,
                    LedgerId = list.BkLedgerId,
                    TranType = "Dr",
                    Amount = request.Amount,
                    DebitAmount = request.Amount,
                    CreditAmount = 0,
                    ShortDesc = "Collection From bKash",
                    PaymentType = "bKash",
                    EntryBy = "bKash",
                    EntryDate = DateTime.Now,
                    IsActive = true
                };

                _ICommonService.Add(debitEntry).Wait();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving bKash payment");
                throw;
            }
        }
        public BkashBillInfo GetBillByTrxId(string UserName, string Password, string TrxId)
        {
            try
            {
                if (UserName != null && Password != null)
                {
                    var user = _userData.GetUser(UserName, Password);

                    if (user == null)
                    {
                        _logger.LogWarning("User authentication failed for: {UserName}", UserName);
                        return null;
                    }

                    _httpContextAccessor.HttpContext?.Session.SetString("TenantId", user.TenantId);

                    string procedur = "SP_GetBillVarifyBYyTrxId";

                    DynamicParameters p = new DynamicParameters();
              
                    p.Add("@QueryChecker", 1);

                    p.Add("@TrxId", TrxId);

                    var billInfo = _IDapperService
                        .GetAllBySP<BkashBillInfo>(procedur, p)
                        .FirstOrDefault();

                    _httpContextAccessor.HttpContext?.Session.Remove("TenantId");

                    return billInfo;
                }

                _logger.LogWarning("Invalid input: UserName or Password is null");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bill information");
                throw;
            }
        }

        public int UpdateBill(ViewModalBill entity)
        {
            try
            {
                decimal voucherAmount = 0;

                foreach (var item in entity.Bills)
                {
                    voucherAmount += Convert.ToDecimal(item.Remarks);

                    _ICommonService.Update<Bill>(item);
                }


                string SP = "Sp_VbnExpense";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 12);
                p.Add("@OwnerCode", entity.FlatCode);
                p.Add("@CollectorCode", entity.UpdateBy);
                p.Add("@VoucherType", "RV");

                var list = _IDapperService.GetAllBySP<VoucherNumber>(SP, p).FirstOrDefault();

                List<VoucherDetails> VoucherDetailsList = new List<VoucherDetails>();

                VoucherDetails details = new VoucherDetails();
                // Crdit
                details.LedgerId = list.LedgerId;
                details.TranType = "Cr";
                details.Amount = voucherAmount;
                details.ShortDesc = "";
                details.EntryBy = entity.UpdateBy;

                VoucherDetailsList.Add(details);
                // for debit

                details = new VoucherDetails();

                details.LedgerId = 12;
                details.TranType = "Dr";
                details.Amount = voucherAmount;
                details.ShortDesc = "Collection From " + (entity.UpdateBy ?? "");
                details.EntryBy = entity.UpdateBy;

                VoucherDetailsList.Add(details);

                string newVoucherNumber = ""; // default
                if (!string.IsNullOrEmpty(list.LastVoucherNumber) && list.LastVoucherNumber.StartsWith("RV#"))
                {
                    var numberPart = list.LastVoucherNumber.Substring(3); // get after "RV#"
                    if (int.TryParse(numberPart, out int num))
                    {
                        newVoucherNumber = $"RV#{num + 1}";
                    }
                }
                else
                {
                    newVoucherNumber = $"RV#1";
                }

                AccVoucher accVoucher = new AccVoucher
                {
                    VoucherType = "RV",
                    VoucherNumber = newVoucherNumber,
                    CompanyId = entity.CompanyId,
                    VoucherDate = entity.Date,
                    Narration = entity.Remarks,
                    TotalAmount = voucherAmount,
                    VoucherStatus = "Pending",
                    EntryBy = entity.UpdateBy,
                    Remarks = entity.BillNo,
                    voucherEntryDetails = VoucherDetailsList

                };
                _ICommonService.Add<AccVoucher>(accVoucher);
                return entity.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion
    }
}
