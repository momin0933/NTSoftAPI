using Azure.Core;
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
        //public BkashBillInfo GetBillMonthWise(string UserName, string Password, string FlatCode, string BillMonth)
        //{
        //    try
        //    {
        //        if (UserName != null && Password != null)
        //        {
        //            // 1. Validate user
        //            var user = _userData.GetUser(UserName, Password);                    

        //            if (user == null)
        //            {
        //                _logger.LogWarning("User authentication failed for: {UserName}", UserName);
        //                return null;
        //            }

        //            // Use IHttpContextAccessor to access the current HttpContext
        //            _httpContextAccessor.HttpContext?.Session.SetString("TenantId", user.TenantId);
        //            string procedur = "SP_GetBillForBkash";
        //            DynamicParameters p = new DynamicParameters();
        //            p.Add("@FlatCode", FlatCode);
        //            p.Add("@BillMonth", BillMonth);
        //            BkashBillInfo billInfo = _IDapperService.GetAllBySP<BkashBillInfo>(procedur, p).FirstOrDefault();
        //            _httpContextAccessor.HttpContext?.Session.Remove("TenantId");                    
        //            return billInfo;
        //        }

        //        _logger.LogWarning("Invalid input: UserName or Password is null");
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting bill information");
        //        throw;
        //    }
        //}


        public BkashBillInfo GetBillMonthWise(string UserName,string Password,string FlatCode,string BillMonth)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserName) ||
                    string.IsNullOrWhiteSpace(Password) ||
                    string.IsNullOrWhiteSpace(FlatCode) ||
                    string.IsNullOrWhiteSpace(BillMonth))
                {
                    return new BkashBillInfo
                    {
                        ErrorCode = "406",
                        ErrorMsg = "Mandatory Field Missing",
                        QueryTime = DateTime.Now.ToString("yyyyMMddHHmmss")
                    };
                }

                // Validate User
                var user = _userData.GetUser(UserName, Password);

                if (user == null)
                {
                    _logger.LogWarning("User authentication failed for: {UserName}", UserName);

                    return new BkashBillInfo
                    {
                        ErrorCode = "403",
                        ErrorMsg = "Authentication failed",
                        QueryTime = DateTime.Now.ToString("yyyyMMddHHmmss")
                    };
                }

                _httpContextAccessor.HttpContext?.Session.SetString("TenantId", user.TenantId);

                string procedure = "SP_GetBillForBkash";

                DynamicParameters p = new DynamicParameters();
                p.Add("@FlatCode", FlatCode);
                p.Add("@BillMonth", BillMonth);

                var billInfo = _IDapperService
                    .GetAllBySP<BkashBillInfo>(procedure, p)
                    .FirstOrDefault();

                _httpContextAccessor.HttpContext?.Session.Remove("TenantId");

                if (billInfo == null)
                {
                    return new BkashBillInfo
                    {
                        ErrorCode = "404",
                        ErrorMsg = "Data Not Found",
                        QueryTime = DateTime.Now.ToString("yyyyMMddHHmmss")
                    };
                }

                // Success Response
                billInfo.ErrorCode = "200";
                billInfo.ErrorMsg = "Bill retrieved successfully";
                billInfo.QueryTime = DateTime.Now.ToString("yyyyMMddHHmmss");
           
                return billInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bill information");

                return new BkashBillInfo
                {
                    ErrorCode = "500",
                    ErrorMsg = "Internal Server Error",
                    QueryTime = DateTime.Now.ToString("yyyyMMddHHmmss")
                };
            }
        }


        public BkashBillPaymentResponse SaveBkashPayment(BkashPaymentRequest request)
        {
            try
            {
                if (request.UserName == null || request.Password == null || request.FlatCode == null || request.BillMonth == null)
                {
                    _logger.LogWarning("Invalid input: UserName or Password is null");

                    return new BkashBillPaymentResponse
                    {
                        ErrorCode = "406",
                        ErrorMsg = "Mandatory Field Missing"
                    };
                }

                var user = _userData.GetUser(request.UserName, request.Password);

                if (user == null)
                {
                    _logger.LogWarning("User authentication failed for: {UserName}", request.UserName);

                    return new BkashBillPaymentResponse
                    {
                        ErrorCode = "403",
                        ErrorMsg = "Authentication failed"
                    };
                }

                _httpContextAccessor.HttpContext?.Session.SetString("TenantId", user.TenantId);

                string sql = @"
            SELECT TOP 1
                b.Id,
                b.BillAmount,
                b.BillNo,
                b.FlatCode,
                fo.OwnerName
            FROM VbntblBill as b
            LEFT JOIN Vw_FlatOwnerInfo fo ON fo.FlatCode = b.FlatCode
            WHERE FORMAT([Date], 'MMyyyy') = @BillMonth
              AND b.FlatCode = @FlatCode";

                var bill = _IDapperService.GetSingle<dynamic>(sql, new
                {
                    BillMonth = request.BillMonth,
                    FlatCode = request.FlatCode
                });

                if (bill == null)
                {
                    _logger.LogWarning("Bill not found for FlatCode: {FlatCode}", request.FlatCode);

                    return new BkashBillPaymentResponse
                    {
                        ErrorCode = "404",
                        ErrorMsg = "Data not found"
                    };
                }

                decimal billAmount = Convert.ToDecimal(bill.BillAmount ?? 0);
                decimal requestAmount = Convert.ToDecimal(request.Amount);

                if (billAmount != requestAmount)
                {
                    _logger.LogWarning("Amount mismatch. DB: {DBAmount}, Request: {ReqAmount}",
                        billAmount, requestAmount);

                    return new BkashBillPaymentResponse
                    {
                        ErrorCode = "409",
                        ErrorMsg = $"Amount mismatch. Bill amount is {billAmount}, but received {requestAmount}"
                    };
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
                    BillId = bill.Id,
                    EntryBy = "bKash",
                    EntryDate = DateTime.Now,
                    IsActive = true
                };

                _ICommonService.Add(payment).Wait();

                string updateSql = @"
            UPDATE VbntblBill
            SET Collection = @Collection,
                CollectionDate = GETDATE(),
                Status = 'Paid',
                UpdateBy = 'Bkash',
                IsActive = 'True'
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

                var list = _IDapperService.GetAllBySP<VoucherNumber>(SP, p).FirstOrDefault();

                if (list == null)
                {
                    _logger.LogWarning("Voucher setup not found");

                    return new BkashBillPaymentResponse
                    {
                        ErrorCode = "404",
                        ErrorMsg = "Voucher setup not found"
                    };
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
                    VoucherStatus = "Pending",
                    CompanyId = 1,
                    Remarks = bill.BillNo,
                    EntryBy = "bKash",
                    EntryDate = DateTime.Now,
                    IsActive = true
                };

                int voucherId = _ICommonService.Add(accVoucher).Result;
           
                _ICommonService.Add(new VoucherDetails
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
                }).Wait();

                _ICommonService.Add(new VoucherDetails
                {
                    AccVoucherId = voucherId,
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
                }).Wait();

                _httpContextAccessor.HttpContext?.Session.Remove("TenantId");

                return new BkashBillPaymentResponse
                {
                    ErrorCode = "200",
                    ErrorMsg = "Successful",
                    ConsumerName = bill.OwnerName,
                    TotalAmount = request.Amount.ToString(),
                    TrxId = request.TrxId,
                    MiddlewarePayTime = DateTime.Now.ToString("yyyyMMddHHmmss")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving bKash payment");

                return new BkashBillPaymentResponse
                {
                    ErrorCode = "500",
                    ErrorMsg = ex.Message
                };
            }
        }


        public BkashBillPaymentResponse GetBillByTrxId(string UserName, string Password, string TrxId)
        {
            try
            {
                // 1. Validate mandatory fields
                if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
                {
                    _logger.LogWarning("Invalid input: UserName or Password is null");

                    return new BkashBillPaymentResponse
                    {
                        ErrorCode = "406",
                        ErrorMsg = "Mandatory Field Missing"
                    };
                }

                // 2. Authenticate user
                var user = _userData.GetUser(UserName, Password);

                if (user == null)
                {
                    _logger.LogWarning("User authentication failed for: {UserName}", UserName);

                    return new BkashBillPaymentResponse
                    {
                        ErrorCode = "403",
                        ErrorMsg = "Authentication failed"
                    };
                }

                _httpContextAccessor.HttpContext?.Session.SetString("TenantId", user.TenantId);

                // 3. Get bill by transaction id
                string procedur = "SP_GetBillVarifyBYyTrxId";

                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@TrxId", TrxId);

                var billInfo = _IDapperService
                    .GetAllBySP<BkashBillInfo>(procedur, p)
                    .FirstOrDefault();

                _httpContextAccessor.HttpContext?.Session.Remove("TenantId");

                // 4. Not found case
                if (billInfo == null)
                {
                    _logger.LogWarning("Bill not found for TrxId: {TrxId}", TrxId);

                    return new BkashBillPaymentResponse
                    {
                        ErrorCode = "404",
                        ErrorMsg = "Data not found"
                    };
                }

                // 5. Success response
                return new BkashBillPaymentResponse
                {
                    ErrorCode = "200",
                    ErrorMsg = "Successful",
                    TotalAmount = billInfo.BillAmount.ToString(),
                    ConsumerName = billInfo.ConsumerName,
                    TrxId = TrxId,
                    MiddlewarePayTime = DateTime.Now.ToString("yyyyMMddHHmmss")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bill information");

                return new BkashBillPaymentResponse
                {
                    ErrorCode = "500",
                    ErrorMsg = ex.Message
                };
            }
        }
        #endregion
    }
}
