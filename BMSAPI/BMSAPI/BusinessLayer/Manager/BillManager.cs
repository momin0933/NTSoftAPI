using BMSAPI.BusinessLayer.Interface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models;
using Dapper;

namespace BMSAPI.BusinessLayer.Manager
{
    public class BillManager : IBill
    {
        private readonly NTSoftDbContextFactory _dbFactory;
        private readonly IDapperService _IDapperService;

        public BillManager(NTSoftDbContextFactory dbFactory, IDapperService dapperService)
        {
            _dbFactory = dbFactory;
            _IDapperService = dapperService;
        }

        public int Add(Bill entity)
        {
            throw new NotImplementedException();
        }

        public int AddBill()
        {
            var p = new DynamicParameters();
            var list = _IDapperService.GetAllBySP<RptBill>("SP_BillGenerate", p).ToList();

            if (list.Count == 0)
                return 0;

            if (list.First().Billstatus == 1)
                return 0;

            var p1 = new DynamicParameters();
            p1.Add("@QueryChecker", 8);
            p1.Add("@CollectorCode", "FlatWiseIncome");
            p1.Add("@VoucherType", "JV");

            var list1 = _IDapperService.GetAllBySP<VoucherNumber>("Sp_VbnExpense", p1).ToList();

            var collectorLedger = list1.FirstOrDefault(x => x.LdgAccNo == "FlatWiseIncome")
                ?? throw new Exception("'FlatWiseIncome' ledger not found.");
            int collectorLedgerId = collectorLedger.LedgerId;

            string lastVoucherNumber = list1.FirstOrDefault()?.LastVoucherNumber ?? "JV#0";

            var voucherListFinal = new List<AccVoucher>();
            int increment = 0;

            foreach (var item in list)
            {
                var ownerItem = list1.FirstOrDefault(x => x.LdgAccNo == item.FlatCode);
                if (ownerItem == null)
                    continue; // owner ledger না পেলে এই flat skip

                var voucherDetailsList = new List<VoucherDetails>
                {
                    // Credit: Flat-wise income
                    new VoucherDetails
                    {
                        LedgerId = collectorLedgerId,
                        TranType = "Cr",
                        Amount = item.BillAmount,
                        ShortDesc = "",
                        EntryBy = item.UpdateBy
                    },
                    // Debit: Owner
                    new VoucherDetails
                    {
                        LedgerId = ownerItem.LedgerId,
                        TranType = "Dr",
                        Amount = item.BillAmount,
                        ShortDesc = "",
                        EntryBy = item.UpdateBy
                    }
                };

                increment++;
                string newVoucherNumber = "JV#1";
                if (lastVoucherNumber.StartsWith("JV#") &&
                    int.TryParse(lastVoucherNumber.Substring(3), out int num))
                {
                    newVoucherNumber = $"JV#{num + increment}";
                }

                voucherListFinal.Add(new AccVoucher
                {
                    VoucherType = "JV",
                    VoucherNumber = newVoucherNumber,
                    CompanyId = 1,
                    VoucherDate = DateTime.Now,
                    Narration = item.Remarks,
                    TotalAmount = item.BillAmount,
                    VoucherStatus = "Pending",
                    EntryBy = item.UpdateBy,
                    Remarks = item.BillNo,
                    voucherEntryDetails = voucherDetailsList
                });
            }

            if (voucherListFinal.Count > 0)
            {
                using var db = _dbFactory.CreateDbContext();
                db.voucherEntryDetails.AddRange(voucherListFinal);
                db.SaveChanges();
            }

            return list.Count;
        }

        public List<Bill> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<RptBill> GetBillByFlatCode(string FlatCode)
        {
            var p = new DynamicParameters();
            p.Add("@QueryChecker", 7);
            p.Add("@FlatCode", FlatCode);
            return _IDapperService.GetAllBySP<RptBill>("Sp_VbnExpense", p).ToList();
        }

        public int Remove(Bill entity)
        {
            throw new NotImplementedException();
        }

        public int Update(Bill entity)
        {
            throw new NotImplementedException();
        }

        public int UpdateBill(ViewModalBill entity)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@QueryChecker", 12);
                p.Add("@OwnerCode", entity.FlatCode);
                p.Add("@CollectorCode", entity.UpdateBy);
                p.Add("@VoucherType", "RV");

                var voucherInfo = _IDapperService.GetAllBySP<VoucherNumber>("Sp_VbnExpense", p).FirstOrDefault()
                    ?? throw new Exception($"Ledger info not found for FlatCode {entity.FlatCode}");

                decimal voucherAmount = entity.Bills.Sum(x => Convert.ToDecimal(x.Remarks));

                var voucherDetailsList = new List<VoucherDetails>
                {
                    // Credit: Owner
                    new VoucherDetails
                    {
                        LedgerId = voucherInfo.LedgerId,
                        TranType = "Cr",
                        Amount = voucherAmount,
                        ShortDesc = "",
                        EntryBy = entity.UpdateBy
                    },
                    // Debit: Cash
                    new VoucherDetails
                    {
                        LedgerId = 12,
                        TranType = "Dr",
                        Amount = voucherAmount,
                        ShortDesc = "Collection By " + (entity.UpdateBy ?? ""),
                        EntryBy = entity.UpdateBy
                    }
                };

                string newVoucherNumber = "RV#1";
                if (!string.IsNullOrEmpty(voucherInfo.LastVoucherNumber) &&
                    voucherInfo.LastVoucherNumber.StartsWith("RV#") &&
                    int.TryParse(voucherInfo.LastVoucherNumber.Substring(3), out int num))
                {
                    newVoucherNumber = $"RV#{num + 1}";
                }

                var accVoucher = new AccVoucher
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
                    voucherEntryDetails = voucherDetailsList
                };

                using var db = _dbFactory.CreateDbContext();
                using var tx = db.Database.BeginTransaction();

                db.Bills.UpdateRange(entity.Bills);
                db.voucherEntryDetails.Add(accVoucher);
                db.SaveChanges();

                tx.Commit();
                return entity.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}