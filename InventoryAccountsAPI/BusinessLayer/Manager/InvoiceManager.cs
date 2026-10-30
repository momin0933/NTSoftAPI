using Dapper;
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.BusinessLayer.Service;
using InventoryAccountsAPI.Models;
using InventoryAccountsAPI.Models.ReportModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAccountsAPI.BusinessLayer.Manager
{
    public class InvoiceManager : IInvoice
    {
        //readonly FTDbContext _dbContext;
        readonly NTSoftDbContext _dbContext;
        private readonly IDapperService _IDapperService;
        ICommonService _ICommonService;
        //public InvoiceManager(FTDbContext dbContext, DapperService dapperService)
        //{
        //    _dbContext = dbContext;
        //    _IDapperService = dapperService;
        //    _ICommonService = new CommonService(_dbContext);
        //}
        public InvoiceManager(NTSoftDbContext dbContext, DapperService dapperService, NTSoftDbContextFactory dbContextFactory)
        {
            _dbContext = dbContext;
            _IDapperService = dapperService;
            _ICommonService = new CommonService(dbContextFactory);
        }
        public int Add(Invoice entity)
        {
            _ICommonService.Add<Invoice>(entity);
            return entity.Id;
        }

        public List<Invoice> GetAll()
        {
            List<Invoice> invoiceList = _ICommonService.GetAll<Invoice>().Where(x => x.IsActive == true).ToList();
            return invoiceList;
        }

        public int Remove(Invoice entity)
        {
            throw new NotImplementedException();
        }

        public int Update(Invoice entity)
        {
            _ICommonService.Update<Invoice>(entity);
            return entity.Id;
        }
        public int AddAndUpdate(InvoicePerams entity)
        {
            int id = 0;
            try
            {

                if (entity != null)
                {
                    var peramsItemList = entity.ItemList.ToList();
                    string paymentTypes = entity.Payments != null && entity.Payments.Any()
                        ? string.Join(", ", entity.Payments.Select(p => p.PaymentType))
                        : "";
                    Invoice invoice = new Invoice
                    {
                        InvoiceNo = entity.InvoiceNo,
                        InvoiceDt = DateTime.Parse(entity.InvoiceDt),
                        WhId = (int)(entity.WhId != null ? entity.WhId : 0),
                        EntryBy = entity.EntryBy,
                        EntryDate = entity.EntryDate,
                        PaymentType = entity.PaymentType ?? paymentTypes,
                        //PaymentType = entity.PaymentType,
                        //PaymentType = paymentTypes,
                        DiscountPercentage = entity.DiscountPercentage,
                        Discount = entity.Discount,
                        IsActive = entity.IsActive,
                        Remarks = entity.Remarks,
                        CustomerNumber = string.IsNullOrWhiteSpace(entity.CustomerNumber) ? null : entity.CustomerNumber,
                        Total = entity.Total,
                        EarningPoint = entity.EarningPoint,
                        RedeemedPoint = entity.RedeemedPoint,
                        Vat = entity.Vat,
                        GiftVoucherDis = entity.GiftVoucherDis,
                        GiftVoucherId = entity.GiftVoucherId,
                        PaidAmount = entity.PaidAmount,
                        SalesMan = entity.SalesMan,
                        Note = entity.Note,
                    };

                    _ICommonService.Add<Invoice>(invoice);
                    id = invoice.Id;
                    foreach (var item in peramsItemList)
                    {
                        decimal qtyAbs = Math.Abs((decimal)item.Qty);
                        // decimal vat = (decimal)(((item.SellingPrice * qtyAbs) - item.Discount - item.PointValue - item.GiftVoucherDis) * 0.10m);


                        InvoiceDetail invoiceDetails = new InvoiceDetail
                        {
                            InvoiceId = id,
                            ItemId = item.ItemId,
                            VariantId = item.VariantId,
                            InvoiceNo = entity.InvoiceNo,
                            InvoiceDt = DateTime.Parse(entity.InvoiceDt),
                            Qty = item.Qty,
                            SellingPrice = item.SellingPrice,
                            CostingPrice = item.CostingPrice,
                            Vat = item.Vat,
                            Discount = item.Discount,
                            DiscountPercentage = item.DiscountPercentage,
                            UnitPrice = item.UnitPrice,
                            EntryBy = entity.EntryBy,
                            EntryDate = entity.EntryDate,
                            IsActive = entity.IsActive,
                            Remarks = item.Remarks,
                            Redeemed_PointValue = item.PointValue,
                            GiftVoucherDis = item.GiftVoucherDis
                        };
                        _ICommonService.Add<InvoiceDetail>(invoiceDetails);

                        var receiveDetails = _dbContext.POStblItemRcvDetails
                        .Where(d => d.ItemId == item.ItemId
                                 && d.VariantId == item.VariantId
                                 && d.RemainingQty > 0
                                 && _dbContext.ItemReceives.Any(r => r.WhId == entity.WhId))
                        .OrderBy(d => d.Id)
                        .ToList();

                        decimal qtyToIssue = (decimal)item.Qty;

                        foreach (var receiveDetail in receiveDetails)
                        {
                            if (qtyToIssue <= 0) break;

                            decimal availableQty = (decimal)receiveDetail.RemainingQty;
                            decimal issueQty = (qtyToIssue >= availableQty) ? availableQty : qtyToIssue;

                            // Update IssueQty
                            var updateQuery = @"UPDATE POStblItemRcvDetails
                    SET IssueQty = IssueQty + @IssueQty, 
                        RemainingQty = RemainingQty - @IssueQty
                    WHERE Id = @Id;";

                            _dbContext.Database.ExecuteSqlRaw(updateQuery,
                                new SqlParameter("@IssueQty", issueQty),
                                new SqlParameter("@Id", receiveDetail.Id));

                            qtyToIssue -= (decimal)issueQty;
                        }

                    }
                    ;

                    foreach (var item in peramsItemList)
                    {

                        var stock = _ICommonService.GetAll<ItemStock>().Where(x => x.ItemId == item.ItemId && x.VariantId == item.VariantId && x.WhId == entity.WhId).FirstOrDefault();
                        if (stock != null)
                        {

                            if (item.Qty < 0)
                            {
                                stock.ReturnQty = stock.ReturnQty == null ? item.Qty * (-1) : stock.ReturnQty + item.Qty * (-1);

                            }
                            else
                            {
                                stock.ReturnQty = stock.ReturnQty;
                                stock.IssueQty = stock.IssueQty == null ? item.Qty : stock.IssueQty + item.Qty;
                            }
                            //stock.StockQty = (stock.ReceiveQty + stock.ReturnQty) - stock.IssueQty;
                            //when item is Physical adjasment, stockQty is get negetive
                            stock.StockQty = stock.StockQty - item.Qty;
                        }
                        _ICommonService.Update<ItemStock>(stock);

                    }
                    ;
                    foreach (var tran in peramsItemList)
                    {
                        string status = "";
                        if (tran.Qty < 0)
                        {
                            status = "Return";

                        }
                        else
                        {
                            status = "Sell";
                        }
                        POStblItemTransaction transaction = new POStblItemTransaction
                        {
                            ItemId = tran.ItemId,
                            VariantId = tran.VariantId,
                            WhId = (int)(entity.WhId != null ? entity.WhId : 0),
                            TranDt = DateTime.Parse(entity.InvoiceDt),
                            Qty = tran.Qty,
                            Status = status,
                            Remarks = tran.Remarks,
                            EntryBy = tran.EntryBy,
                            EntryDate = tran.EntryDate,
                            IsActive = tran.IsActive
                        };
                        _ICommonService.Add<POStblItemTransaction>(transaction);
                    }


                    //CustomerPayment customerPayment = new CustomerPayment
                    //{
                    //    CustomerMobile = entity.CustomerNumber,
                    //    InvoiceId = id,
                    //    PaymentDate = DateTime.Parse(entity.InvoiceDt),
                    //    PaidAmount = entity.PaidAmount,
                    //    // PaymentType = entity.PaymentType,
                    //    EntryBy = entity.EntryBy,
                    //    EntryDate = entity.EntryDate,
                    //    IsActive = entity.IsActive,
                    //    AccountId = (int)entity.AccountId

                    //};
                    //_ICommonService.Add<CustomerPayment>(customerPayment);


                    //CashBankLedger cashBank = new CashBankLedger
                    //{
                    //    TranDate = DateTime.Parse(entity.InvoiceDt),
                    //    SourceId = customerPayment.Id,
                    //    SourceType = "Invoice",
                    //    LedgerType = "Income",
                    //    Amount = entity.PaidAmount,
                    //    TransactionType = "In",
                    //    EntryBy = entity.EntryBy,
                    //    EntryDate = entity.EntryDate,
                    //    IsActive = entity.IsActive,
                    //    AccountId = (int)entity.AccountId,
                    //    LedgerId = (int)entity.LedgerId
                    //};
                    //_ICommonService.Add<CashBankLedger>(cashBank);

                    if (entity.Payments != null && entity.Payments.Any())
                    {
                        foreach (var p in entity.Payments)
                        {
                            CustomerPayment customerPayment = new CustomerPayment
                            {
                                CustomerMobile = entity.CustomerNumber,
                                InvoiceId = id,
                                PaymentDate = DateTime.Parse(entity.InvoiceDt),
                                PaidAmount = p.Amount, // each row amount
                                EntryBy = entity.EntryBy,
                                EntryDate = entity.EntryDate,
                                IsActive = entity.IsActive,
                                AccountId = p.AccountId
                            };
                            _ICommonService.Add<CustomerPayment>(customerPayment);

                            CashBankLedger cashBank = new CashBankLedger
                            {
                                TranDate = DateTime.Parse(entity.InvoiceDt),
                                SourceId = customerPayment.Id,
                                SourceType = "Invoice",
                                LedgerType = "Income",
                                Amount = p.Amount, // each row amount
                                TransactionType = "In",
                                EntryBy = entity.EntryBy,
                                EntryDate = entity.EntryDate,
                                IsActive = entity.IsActive,
                                AccountId = p.AccountId,
                                LedgerId = (int)entity.LedgerId // same ledger or map from p if needed
                            };
                            _ICommonService.Add<CashBankLedger>(cashBank);
                        }
                    }
                    else
                    {
                        // Single-payment scenario (Payments is null)
                        decimal paidAmount = entity.PaidAmount ?? 0;
                        int accountId = entity.AccountId ?? 0;

                        CustomerPayment customerPayment = new CustomerPayment
                        {
                            CustomerMobile = entity.CustomerNumber,
                            InvoiceId = id,
                            PaymentDate = DateTime.Parse(entity.InvoiceDt),
                            PaidAmount = paidAmount,
                            EntryBy = entity.EntryBy,
                            EntryDate = entity.EntryDate,
                            IsActive = entity.IsActive,
                            AccountId = accountId
                        };

                        _ICommonService.Add<CustomerPayment>(customerPayment);

                        CashBankLedger cashBank = new CashBankLedger
                        {
                            TranDate = DateTime.Parse(entity.InvoiceDt),
                            SourceId = customerPayment.Id,
                            SourceType = "Invoice",
                            LedgerType = "Income",
                            Amount = paidAmount,
                            TransactionType = "In",
                            EntryBy = entity.EntryBy,
                            EntryDate = entity.EntryDate,
                            IsActive = entity.IsActive,
                            AccountId = accountId,
                            LedgerId = entity.LedgerId ?? 0
                        };

                        _ICommonService.Add<CashBankLedger>(cashBank);
                    }

                    var existingList = _dbContext.GiftVoucherDetails
                         .AsNoTracking()
                         .FirstOrDefault(g => g.Id == entity.GiftVoucherId);

                    if (existingList != null)
                    {
                        GiftVoucherDetails giftVoucherDetails = new GiftVoucherDetails
                        {
                            Id = existingList.Id,
                            GiftVoucherId = existingList.GiftVoucherId,
                            CVName = existingList.CVName,
                            Status = "Close",
                            EntryBy = existingList.EntryBy,
                            EntryDate = existingList.EntryDate,
                            UpdateBy = entity.EntryBy,
                            UpdateDate = entity.UpdateDate,
                            IsActive = existingList.IsActive,
                            InvoiceNo = entity.InvoiceNo
                        };

                        _ICommonService.Update<GiftVoucherDetails>(giftVoucherDetails);
                    }


                    if (string.IsNullOrWhiteSpace(entity.CustomerNumber))
                    {
                        return 0;
                    }

                    var existingCustomer = _dbContext.POStblCustomers.FirstOrDefault(c => c.PhoneNo == entity.CustomerNumber);
                    //  Get membership based on PaidAmount (Single Invoice ভিত্তিক)
                    var membership = _dbContext.CustomerMemberships
                        .Where(m => m.IsActive == true
                            && m.AccessKey == "LM"
                            && entity.PaidAmount >= m.Rule1
                            && entity.PaidAmount <= m.Rule2)
                        .OrderByDescending(m => m.Rule1)
                        .FirstOrDefault();

                    if (existingCustomer != null)
                    {
                        // Ensure default values for Reward_Points and Earn_Points if they are null
                        existingCustomer.Reward_Points ??= 0;
                        existingCustomer.Earn_Points ??= 0;
                        existingCustomer.UpdateBy = entity.EntryBy;
                        existingCustomer.UpdateDate = entity.UpdateDate;
                        existingCustomer.Redeemed_Points += entity.RedeemedPoint;
                        existingCustomer.Earn_Points += entity.EarningPoint;
                        existingCustomer.Reward_Points += entity.EarningPoint - entity.RedeemedPoint;
                        if (membership != null)
                        {
                            existingCustomer.MemberTypeId = membership.Id;
                            existingCustomer.Discount = membership.Discount;
                        }
                        // Determine membership type based on Reward_Points
                        _ICommonService.Update<POStblCustomer>(existingCustomer);
                    }
                    else
                    {
                        POStblCustomer customer = new POStblCustomer
                        {
                            EntryBy = entity.EntryBy,
                            EntryDate = entity.EntryDate,
                            PhoneNo = entity.CustomerNumber,
                            Reward_Points = entity.EarningPoint,
                            Earn_Points = entity.EarningPoint,
                            Redeemed_Points = 0,
                            MemberTypeId = membership?.Id,
                            Discount = membership?.Discount ?? 0
                        };
                        _ICommonService.Add<POStblCustomer>(customer);
                    }



                }
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }

        }

        //public int AddAndUpdateNew(InvoicePerams entity)
        //{
        //    int id = 0;
        //    try
        //    {

        //        if (entity != null)
        //        {
        //            var peramsItemList = entity.ItemList.ToList();
        //            string paymentTypes = entity.Payments != null && entity.Payments.Any()
        //                ? string.Join(", ", entity.Payments.Select(p => p.PaymentType))
        //                : "";
        //            Invoice invoice = new Invoice
        //            {
        //                InvoiceNo = entity.InvoiceNo,
        //                InvoiceDt = DateTime.Parse(entity.InvoiceDt),
        //                WhId = (int)(entity.WhId != null ? entity.WhId : 0),
        //                EntryBy = entity.EntryBy,
        //                EntryDate = entity.EntryDate,
        //                PaymentType = entity.PaymentType ?? paymentTypes,
        //                //PaymentType = entity.PaymentType,
        //                //PaymentType = paymentTypes,
        //                DiscountPercentage = entity.DiscountPercentage,
        //                Discount = entity.Discount,
        //                IsActive = entity.IsActive,
        //                Remarks = entity.Remarks,
        //                CustomerNumber = string.IsNullOrWhiteSpace(entity.CustomerNumber) ? null : entity.CustomerNumber,
        //                Total = entity.Total,
        //                EarningPoint = entity.EarningPoint,
        //                RedeemedPoint = entity.RedeemedPoint,
        //                Vat = entity.Vat,
        //                GiftVoucherDis = entity.GiftVoucherDis,
        //                GiftVoucherId = entity.GiftVoucherId,
        //                PaidAmount = entity.PaidAmount,
        //                SalesMan = entity.SalesMan,
        //                Note = entity.Note,
        //            };

        //            _ICommonService.Add<Invoice>(invoice);
        //            id = invoice.Id;
        //            foreach (var item in peramsItemList)
        //            {
        //                decimal qtyAbs = Math.Abs((decimal)item.Qty);
        //                // decimal vat = (decimal)(((item.SellingPrice * qtyAbs) - item.Discount - item.PointValue - item.GiftVoucherDis) * 0.10m);


        //                InvoiceDetail invoiceDetails = new InvoiceDetail
        //                {
        //                    InvoiceId = id,
        //                    ItemId = item.ItemId,
        //                    VariantId = item.VariantId,
        //                    InvoiceNo = entity.InvoiceNo,
        //                    InvoiceDt = DateTime.Parse(entity.InvoiceDt),
        //                    Qty = item.Qty,
        //                    SellingPrice = item.SellingPrice,
        //                    CostingPrice = item.CostingPrice,
        //                    Vat = item.Vat,
        //                    Discount = item.Discount,
        //                    DiscountPercentage = item.DiscountPercentage,
        //                    UnitPrice = item.UnitPrice,
        //                    EntryBy = entity.EntryBy,
        //                    EntryDate = entity.EntryDate,
        //                    IsActive = entity.IsActive,
        //                    Remarks = item.Remarks,
        //                    Redeemed_PointValue = item.PointValue,
        //                    GiftVoucherDis = item.GiftVoucherDis
        //                };
        //                _ICommonService.Add<InvoiceDetail>(invoiceDetails);

        //                var receiveDetails = _dbContext.POStblItemRcvDetails
        //                .Where(d => d.ItemId == item.ItemId
        //                         && d.VariantId == item.VariantId
        //                         && d.RemainingQty > 0
        //                         && _dbContext.ItemReceives.Any(r => r.WhId == entity.WhId))
        //                .OrderBy(d => d.Id)
        //                .ToList();

        //                decimal qtyToIssue = (decimal)item.Qty;

        //                foreach (var receiveDetail in receiveDetails)
        //                {
        //                    if (qtyToIssue <= 0) break;

        //                    decimal availableQty = (decimal)receiveDetail.RemainingQty;
        //                    decimal issueQty = (qtyToIssue >= availableQty) ? availableQty : qtyToIssue;

        //                    // Update IssueQty
        //                    var updateQuery = @"UPDATE POStblItemRcvDetails
        //            SET IssueQty = IssueQty + @IssueQty, 
        //                RemainingQty = RemainingQty - @IssueQty
        //            WHERE Id = @Id;";

        //                    _dbContext.Database.ExecuteSqlRaw(updateQuery,
        //                        new SqlParameter("@IssueQty", issueQty),
        //                        new SqlParameter("@Id", receiveDetail.Id));

        //                    qtyToIssue -= (decimal)issueQty;
        //                }

        //            }
        //            ;

        //            foreach (var item in peramsItemList)
        //            {

        //                var stock = _ICommonService.GetAll<ItemStock>().Where(x => x.ItemId == item.ItemId && x.VariantId == item.VariantId && x.WhId == entity.WhId).FirstOrDefault();
        //                if (stock != null)
        //                {

        //                    if (item.Qty < 0)
        //                    {
        //                        stock.ReturnQty = stock.ReturnQty == null ? item.Qty * (-1) : stock.ReturnQty + item.Qty * (-1);

        //                    }
        //                    else
        //                    {
        //                        stock.ReturnQty = stock.ReturnQty;
        //                        stock.IssueQty = stock.IssueQty == null ? item.Qty : stock.IssueQty + item.Qty;
        //                    }
        //                    //stock.StockQty = (stock.ReceiveQty + stock.ReturnQty) - stock.IssueQty;
        //                    //when item is Physical adjasment, stockQty is get negetive
        //                    stock.StockQty = stock.StockQty - item.Qty;
        //                }
        //                _ICommonService.Update<ItemStock>(stock);

        //            }
        //            ;
        //            foreach (var tran in peramsItemList)
        //            {
        //                string status = "";
        //                if (tran.Qty < 0)
        //                {
        //                    status = "Return";

        //                }
        //                else
        //                {
        //                    status = "Sell";
        //                }
        //                POStblItemTransaction transaction = new POStblItemTransaction
        //                {
        //                    ItemId = tran.ItemId,
        //                    VariantId = tran.VariantId,
        //                    WhId = (int)(entity.WhId != null ? entity.WhId : 0),
        //                    TranDt = DateTime.Parse(entity.InvoiceDt),
        //                    Qty = tran.Qty,
        //                    Status = status,
        //                    Remarks = tran.Remarks,
        //                    EntryBy = tran.EntryBy,
        //                    EntryDate = tran.EntryDate,
        //                    IsActive = tran.IsActive
        //                };
        //                _ICommonService.Add<POStblItemTransaction>(transaction);
        //            }

        //            if (entity.Payments != null && entity.Payments.Any())
        //            {
        //                foreach (var p in entity.Payments)
        //                {
        //                    CustomerPayment customerPayment = new CustomerPayment
        //                    {
        //                        CustomerMobile = entity.CustomerNumber,
        //                        InvoiceId = id,
        //                        PaymentDate = DateTime.Parse(entity.InvoiceDt),
        //                        PaidAmount = p.Amount, // each row amount
        //                        EntryBy = entity.EntryBy,
        //                        EntryDate = entity.EntryDate,
        //                        IsActive = entity.IsActive,
        //                        AccountId = p.AccountId
        //                    };
        //                    _ICommonService.Add<CustomerPayment>(customerPayment);

        //                }
        //            }
        //            else
        //            {
        //                // Single-payment scenario (Payments is null)
        //                decimal paidAmount = entity.PaidAmount ?? 0;
        //                int accountId = entity.AccountId ?? 0;

        //                CustomerPayment customerPayment = new CustomerPayment
        //                {
        //                    CustomerMobile = entity.CustomerNumber,
        //                    InvoiceId = id,
        //                    PaymentDate = DateTime.Parse(entity.InvoiceDt),
        //                    PaidAmount = paidAmount,
        //                    EntryBy = entity.EntryBy,
        //                    EntryDate = entity.EntryDate,
        //                    IsActive = entity.IsActive,
        //                    AccountId = accountId
        //                };

        //                _ICommonService.Add<CustomerPayment>(customerPayment);

        //                //CashBankLedger cashBank = new CashBankLedger
        //                //{
        //                //    TranDate = DateTime.Parse(entity.InvoiceDt),
        //                //    SourceId = customerPayment.Id,
        //                //    SourceType = "Invoice",
        //                //    LedgerType = "Income",
        //                //    Amount = paidAmount,
        //                //    TransactionType = "In",
        //                //    EntryBy = entity.EntryBy,
        //                //    EntryDate = entity.EntryDate,
        //                //    IsActive = entity.IsActive,
        //                //    AccountId = accountId,
        //                //    LedgerId = entity.LedgerId ?? 0
        //                //};

        //                //_ICommonService.Add<CashBankLedger>(cashBank);
        //            }

        //            var existingList = _dbContext.GiftVoucherDetails
        //                 .AsNoTracking()
        //                 .FirstOrDefault(g => g.Id == entity.GiftVoucherId);

        //            if (existingList != null)
        //            {
        //                GiftVoucherDetails giftVoucherDetails = new GiftVoucherDetails
        //                {
        //                    Id = existingList.Id,
        //                    GiftVoucherId = existingList.GiftVoucherId,
        //                    CVName = existingList.CVName,
        //                    Status = "Close",
        //                    EntryBy = existingList.EntryBy,
        //                    EntryDate = existingList.EntryDate,
        //                    UpdateBy = entity.EntryBy,
        //                    UpdateDate = entity.UpdateDate,
        //                    IsActive = existingList.IsActive,
        //                    InvoiceNo = entity.InvoiceNo
        //                };

        //                _ICommonService.Update<GiftVoucherDetails>(giftVoucherDetails);
        //            }


        //            if (string.IsNullOrWhiteSpace(entity.CustomerNumber))
        //            {
        //                return 0;
        //            }

        //            var existingCustomer = _dbContext.POStblCustomers.FirstOrDefault(c => c.PhoneNo == entity.CustomerNumber);
        //            //  Get membership based on PaidAmount (Single Invoice ভিত্তিক)
        //            var membership = _dbContext.CustomerMemberships
        //                .Where(m => m.IsActive == true
        //                    && m.AccessKey == "LM"
        //                    && entity.PaidAmount >= m.Rule1
        //                    && entity.PaidAmount <= m.Rule2)
        //                .OrderByDescending(m => m.Rule1)
        //                .FirstOrDefault();

        //            if (existingCustomer != null)
        //            {
        //                // Ensure default values for Reward_Points and Earn_Points if they are null
        //                existingCustomer.Reward_Points ??= 0;
        //                existingCustomer.Earn_Points ??= 0;
        //                existingCustomer.UpdateBy = entity.EntryBy;
        //                existingCustomer.UpdateDate = entity.UpdateDate;
        //                existingCustomer.Redeemed_Points += entity.RedeemedPoint;
        //                existingCustomer.Earn_Points += entity.EarningPoint;
        //                existingCustomer.Reward_Points += entity.EarningPoint - entity.RedeemedPoint;
        //                if (membership != null)
        //                {
        //                    existingCustomer.MemberTypeId = membership.Id;
        //                    existingCustomer.Discount = membership.Discount;
        //                }
        //                // Determine membership type based on Reward_Points
        //                _ICommonService.Update<POStblCustomer>(existingCustomer);
        //            }
        //            else
        //            {
        //                POStblCustomer customer = new POStblCustomer
        //                {
        //                    EntryBy = entity.EntryBy,
        //                    EntryDate = entity.EntryDate,
        //                    PhoneNo = entity.CustomerNumber,
        //                    Reward_Points = entity.EarningPoint,
        //                    Earn_Points = entity.EarningPoint,
        //                    Redeemed_Points = 0,
        //                    MemberTypeId = membership?.Id,
        //                    Discount = membership?.Discount ?? 0
        //                };
        //                _ICommonService.Add<POStblCustomer>(customer);
        //            }



        //        }
        //        return id;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        throw ex;
        //    }

        //}

        public async Task<int> AddAndUpdateNew(InvoicePerams entity)
        {
            int id = 0;
            try
            {

                if (entity != null)
                {
                    var peramsItemList = entity.ItemList.ToList();
                    string paymentTypes = entity.Payments != null && entity.Payments.Any()
                        ? string.Join(", ", entity.Payments.Select(p => p.PaymentType))
                        : "";
                    Invoice invoice = new Invoice
                    {
                        InvoiceNo = entity.InvoiceNo,
                        InvoiceDt = DateTime.Parse(entity.InvoiceDt),
                        WhId = (int)(entity.WhId != null ? entity.WhId : 0),
                        EntryBy = entity.EntryBy,
                        EntryDate = entity.EntryDate,
                        PaymentType = entity.PaymentType ?? paymentTypes,
                        //PaymentType = entity.PaymentType,
                        //PaymentType = paymentTypes,
                        DiscountPercentage = entity.DiscountPercentage,
                        Discount = entity.Discount,
                        IsActive = entity.IsActive,
                        Remarks = entity.Remarks,
                        CustomerNumber = string.IsNullOrWhiteSpace(entity.CustomerNumber) ? null : entity.CustomerNumber,
                        Total = entity.Total,
                        EarningPoint = entity.EarningPoint,
                        RedeemedPoint = entity.RedeemedPoint,
                        Vat = entity.Vat,
                        GiftVoucherDis = entity.GiftVoucherDis,
                        GiftVoucherId = entity.GiftVoucherId,
                        PaidAmount = entity.PaidAmount,
                        SalesMan = entity.SalesMan,
                        Note = entity.Note,
                    };

                    await _ICommonService.Add<Invoice>(invoice);
                    id = invoice.Id;
                    foreach (var item in peramsItemList)
                    {
                        decimal qtyAbs = Math.Abs((decimal)item.Qty);
                        // decimal vat = (decimal)(((item.SellingPrice * qtyAbs) - item.Discount - item.PointValue - item.GiftVoucherDis) * 0.10m);


                        InvoiceDetail invoiceDetails = new InvoiceDetail
                        {
                            InvoiceId = id,
                            ItemId = item.ItemId,
                            VariantId = item.VariantId,
                            InvoiceNo = entity.InvoiceNo,
                            InvoiceDt = DateTime.Parse(entity.InvoiceDt),
                            Qty = item.Qty,
                            SellingPrice = item.SellingPrice,
                            CostingPrice = item.CostingPrice,
                            Vat = item.Vat,
                            Discount = item.Discount,
                            DiscountPercentage = item.DiscountPercentage,
                            UnitPrice = item.UnitPrice,
                            EntryBy = entity.EntryBy,
                            EntryDate = entity.EntryDate,
                            IsActive = entity.IsActive,
                            Remarks = item.Remarks,
                            Redeemed_PointValue = item.PointValue,
                            GiftVoucherDis = item.GiftVoucherDis
                        };
                        await _ICommonService.Add<InvoiceDetail>(invoiceDetails);

                        var receiveDetails = _dbContext.POStblItemRcvDetails
                        .Where(d => d.ItemId == item.ItemId
                                 && d.VariantId == item.VariantId
                                 && d.RemainingQty > 0
                                 && _dbContext.ItemReceives.Any(r => r.WhId == entity.WhId))
                        .OrderBy(d => d.Id)
                        .ToList();

                        decimal qtyToIssue = (decimal)item.Qty;

                        foreach (var receiveDetail in receiveDetails)
                        {
                            if (qtyToIssue <= 0) break;

                            decimal availableQty = (decimal)receiveDetail.RemainingQty;
                            decimal issueQty = (qtyToIssue >= availableQty) ? availableQty : qtyToIssue;

                            // Update IssueQty
                            var updateQuery = @"UPDATE POStblItemRcvDetails
            SET IssueQty = IssueQty + @IssueQty, 
                RemainingQty = RemainingQty - @IssueQty
            WHERE Id = @Id;";

                            _dbContext.Database.ExecuteSqlRaw(updateQuery,
                                new SqlParameter("@IssueQty", issueQty),
                                new SqlParameter("@Id", receiveDetail.Id));

                            qtyToIssue -= (decimal)issueQty;
                        }

                    }
                    ;

                    foreach (var item in peramsItemList)
                    {

                        var stock = _ICommonService.GetAll<ItemStock>().Where(x => x.ItemId == item.ItemId && x.VariantId == item.VariantId && x.WhId == entity.WhId).FirstOrDefault();
                        if (stock != null)
                        {

                            if (item.Qty < 0)
                            {
                                stock.ReturnQty = stock.ReturnQty == null ? item.Qty * (-1) : stock.ReturnQty + item.Qty * (-1);

                            }
                            else
                            {
                                stock.ReturnQty = stock.ReturnQty;
                                stock.IssueQty = stock.IssueQty == null ? item.Qty : stock.IssueQty + item.Qty;
                            }
                            //stock.StockQty = (stock.ReceiveQty + stock.ReturnQty) - stock.IssueQty;
                            //when item is Physical adjasment, stockQty is get negetive
                            stock.StockQty = stock.StockQty - item.Qty;
                        }
                        _ICommonService.Update<ItemStock>(stock);

                    }
                    ;
                    foreach (var tran in peramsItemList)
                    {
                        string status = "";
                        if (tran.Qty < 0)
                        {
                            status = "Return";

                        }
                        else
                        {
                            status = "Sell";
                        }
                        POStblItemTransaction transaction = new POStblItemTransaction
                        {
                            ItemId = tran.ItemId,
                            VariantId = tran.VariantId,
                            WhId = (int)(entity.WhId != null ? entity.WhId : 0),
                            TranDt = DateTime.Parse(entity.InvoiceDt),
                            Qty = tran.Qty,
                            Status = status,
                            Remarks = tran.Remarks,
                            EntryBy = tran.EntryBy,
                            EntryDate = tran.EntryDate,
                            IsActive = tran.IsActive
                        };
                        await _ICommonService.Add<POStblItemTransaction>(transaction);
                    }

                    if (entity.Payments != null && entity.Payments.Any())
                    {
                        foreach (var p in entity.Payments)
                        {
                            CustomerPayment customerPayment = new CustomerPayment
                            {
                                CustomerMobile = entity.CustomerNumber,
                                InvoiceId = id,
                                PaymentDate = DateTime.Parse(entity.InvoiceDt),
                                PaidAmount = p.Amount, // each row amount
                                EntryBy = entity.EntryBy,
                                EntryDate = entity.EntryDate,
                                IsActive = entity.IsActive,
                                AccountId = p.AccountId
                            };
                            await _ICommonService.Add<CustomerPayment>(customerPayment);

                        }
                    }
                    else
                    {
                        // Single-payment scenario (Payments is null)
                        decimal paidAmount = entity.PaidAmount ?? 0;
                        int accountId = entity.AccountId ?? 0;

                        CustomerPayment customerPayment = new CustomerPayment
                        {
                            CustomerMobile = entity.CustomerNumber,
                            InvoiceId = id,
                            PaymentDate = DateTime.Parse(entity.InvoiceDt),
                            PaidAmount = paidAmount,
                            EntryBy = entity.EntryBy,
                            EntryDate = entity.EntryDate,
                            IsActive = entity.IsActive,
                            AccountId = accountId
                        };

                        await _ICommonService.Add<CustomerPayment>(customerPayment);

                        //CashBankLedger cashBank = new CashBankLedger
                        //{
                        //    TranDate = DateTime.Parse(entity.InvoiceDt),
                        //    SourceId = customerPayment.Id,
                        //    SourceType = "Invoice",
                        //    LedgerType = "Income",
                        //    Amount = paidAmount,
                        //    TransactionType = "In",
                        //    EntryBy = entity.EntryBy,
                        //    EntryDate = entity.EntryDate,
                        //    IsActive = entity.IsActive,
                        //    AccountId = accountId,
                        //    LedgerId = entity.LedgerId ?? 0
                        //};

                        //await _ICommonService.Add<CashBankLedger>(cashBank);
                    }

                    var existingList = _dbContext.GiftVoucherDetails
                         .AsNoTracking()
                         .FirstOrDefault(g => g.Id == entity.GiftVoucherId);

                    if (existingList != null)
                    {
                        GiftVoucherDetails giftVoucherDetails = new GiftVoucherDetails
                        {
                            Id = existingList.Id,
                            GiftVoucherId = existingList.GiftVoucherId,
                            CVName = existingList.CVName,
                            Status = "Close",
                            EntryBy = existingList.EntryBy,
                            EntryDate = existingList.EntryDate,
                            UpdateBy = entity.EntryBy,
                            UpdateDate = entity.UpdateDate,
                            IsActive = existingList.IsActive,
                            InvoiceNo = entity.InvoiceNo
                        };

                        _ICommonService.Update<GiftVoucherDetails>(giftVoucherDetails);
                    }


                    if (string.IsNullOrWhiteSpace(entity.CustomerNumber))
                    {
                        return 0;
                    }

                    var existingCustomer = _dbContext.POStblCustomers.FirstOrDefault(c => c.PhoneNo == entity.CustomerNumber);
                    //  Get membership based on PaidAmount (Single Invoice ভিত্তিক)
                    var membership = _dbContext.CustomerMemberships
                        .Where(m => m.IsActive == true
                            && m.AccessKey == "LM"
                            && entity.PaidAmount >= m.Rule1
                            && entity.PaidAmount <= m.Rule2)
                        .OrderByDescending(m => m.Rule1)
                        .FirstOrDefault();

                    if (existingCustomer != null)
                    {
                        // Ensure default values for Reward_Points and Earn_Points if they are null
                        existingCustomer.Reward_Points ??= 0;
                        existingCustomer.Earn_Points ??= 0;
                        existingCustomer.UpdateBy = entity.EntryBy;
                        existingCustomer.UpdateDate = entity.UpdateDate;
                        existingCustomer.Redeemed_Points += entity.RedeemedPoint;
                        existingCustomer.Earn_Points += entity.EarningPoint;
                        existingCustomer.Reward_Points += entity.EarningPoint - entity.RedeemedPoint;
                        if (membership != null)
                        {
                            existingCustomer.MemberTypeId = membership.Id;
                            existingCustomer.Discount = membership.Discount;
                        }
                        // Determine membership type based on Reward_Points
                        _ICommonService.Update<POStblCustomer>(existingCustomer);
                    }
                    else
                    {
                        POStblCustomer customer = new POStblCustomer
                        {
                            EntryBy = entity.EntryBy,
                            EntryDate = entity.EntryDate,
                            PhoneNo = entity.CustomerNumber,
                            Reward_Points = entity.EarningPoint,
                            Earn_Points = entity.EarningPoint,
                            Redeemed_Points = 0,
                            MemberTypeId = membership?.Id,
                            Discount = membership?.Discount ?? 0
                        };
                        await _ICommonService.Add<POStblCustomer>(customer);
                    }



                }
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }
        public string AddCustomerPhoneNo(POStblCustomer customer)
        {
            try
            {
                string responseMsg = "";
                if (customer != null)
                {
                    var checkExistance = _ICommonService.GetAll<POStblCustomer>().Where(x => x.PhoneNo == customer.PhoneNo).ToList();
                    if (checkExistance.Count == 0)
                    {
                        _ICommonService.Add<POStblCustomer>(customer);

                        if (!string.IsNullOrEmpty(customer.CusCode))
                        {
                            var customerGroupLedger = _ICommonService
                                .GetAll<AccILedger>()
                                .FirstOrDefault(x => x.Title == "Customer");

                            if (customerGroupLedger == null)
                                throw new Exception("Customer group ledger not found");

                            AccILedger customerLedger = new AccILedger
                            {
                                CompCode = customerGroupLedger.CompCode,
                                GLedgerId = customerGroupLedger.GLedgerId,
                                Code = customer.CusCode,
                                Title = customer.Name,
                                ParentId = customerGroupLedger.Id,
                                OPBalance = 0,
                                PaymentType = "Cash",
                                EntryBy = customer.EntryBy,
                                EntryDate = DateTime.Now,
                                IsActive = true
                            };

                            _ICommonService.Add<AccILedger>(customerLedger);
                        }


                        responseMsg = "Customer Added Sucessfully";
                    }
                    else
                    {
                        responseMsg = "Customer Already exists";
                    }

                }
                return responseMsg;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<POStblCustomer> CustomerDetails(string? phoneNo)
        {
            if (phoneNo == "0")
            {
                //List<POStblCustomer> customerList = _ICommonService.GetAll<POStblCustomer>().ToList();
                var customerList = _ICommonService.Query<POStblCustomer>()
                    .Include(c => c.Ledger)   
                    .ToList();

              
                return customerList;
            }
            else
            {
                string SP = "SP_SalesInvoice";
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 3);
                p.Add("@phoneNo", phoneNo);
                List<POStblCustomer> customerList = _IDapperService.GetAllBySP<POStblCustomer>(SP, p).ToList();
                return customerList;
            }

        }

        //public List<POStblCustomer> CustomerDetails(string? query)
        //{
        //    if (string.IsNullOrWhiteSpace(query) || query == "0")
        //    {
        //        // Return all customers if query is empty or "0"
        //        return _ICommonService.GetAll<POStblCustomer>().ToList();
        //    }
        //    else
        //    {
        //        string SP = "SP_SalesInvoice";
        //        DynamicParameters p = new DynamicParameters();
        //        p.Add("@QueryChecker", 3);
        //        p.Add("@Query", query); 

        //        // Call SP
        //        List<POStblCustomer> customerList = _IDapperService.GetAllBySP<POStblCustomer>(SP, p).ToList();
        //        return customerList;
        //    }
        //}

        public List<CustomerMembership> CustomerMembershipRule()
        {
            try
            {
                var list = new List<CustomerMembership>();
                list = _IDapperService.GetAllByQuery<CustomerMembership>("select Rule1,Rule2 from POStblCustomerMembership where AccessKey = 'P' ").ToList();
                return list;
            }
            catch
            {
                throw;
            }
        }

        public Invoice GetInvoiceInfoByInvoiceNo(string invoiceNo)
        {
            var invoice = _dbContext.Invoices
                .Include(x => x.InvoiceDetails)   // Navigation property name
                .Where(x => x.InvoiceNo == invoiceNo)
                .FirstOrDefault();

            return invoice;
        }

        public int SubmitPayment(PaymentPerams entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            int createdCustomerPaymentId = 0;

            try
            {
                // Ensure PaymentDate is valid
                DateTime paymentDate;
                if (!DateTime.TryParse(entity.PaymentDate, out paymentDate))
                {
                    // fallback to EntryDate or now
                    paymentDate = entity.EntryDate ?? DateTime.Now;
                }

                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        // 1) Find invoice: prefer InvoiceId if provided, otherwise InvoiceNo lookup
                        Invoice invoice = null;
                        if (entity.InvoiceId.HasValue && entity.InvoiceId.Value > 0)
                        {
                            invoice = _dbContext.Set<Invoice>().FirstOrDefault(i => i.Id == entity.InvoiceId.Value);
                        }
                        else if (!string.IsNullOrEmpty(entity.InvoiceNo))
                        {
                            invoice = _dbContext.Set<Invoice>().FirstOrDefault(i => i.InvoiceNo == entity.InvoiceNo);
                        }

                        if (invoice == null)
                            throw new Exception($"Invoice not found. InvoiceNo: {entity.InvoiceNo}, InvoiceId: {entity.InvoiceId}");

                        // 2) Update invoice paid amount (Previous + New)
                        decimal currentPaid = invoice.PaidAmount ?? 0m;
                        decimal newPayment = entity.PaidAmount ?? 0m;
                        invoice.PaidAmount = currentPaid + newPayment;
                        invoice.UpdateBy = entity.EntryBy;
                        invoice.UpdateDate = entity.EntryDate ?? DateTime.Now;

                        _dbContext.Set<Invoice>().Update(invoice);
                        _dbContext.SaveChanges(); // persist invoice update

                        // 3) Insert into POStblCustomerPayment
                        var customerPayment = new CustomerPayment
                        {
                            CustomerMobile = invoice.CustomerNumber,
                            InvoiceId = invoice.Id,
                            PaymentDate = paymentDate,
                            PaidAmount = newPayment,
                            AccountId = entity.PaymentTypeId, // set if you have an account mapping
                            Remarks = entity.PaymentType, // optional
                            EntryBy = entity.EntryBy,
                            EntryDate = entity.EntryDate ?? DateTime.Now,
                            UpdateBy = null,
                            UpdateDate = null,
                            IsActive = true
                        };

                        _dbContext.Set<CustomerPayment>().Add(customerPayment);
                        _dbContext.SaveChanges(); 
                        createdCustomerPaymentId = customerPayment.Id;

                        // 4) Insert into POStblCashBankLedger
                        var cashBankLedger = new CashBankLedger
                        {
                            TranDate = paymentDate,
                            SourceId = customerPayment.Id,
                            SourceType = "Invoice",
                            LedgerType = "Income",
                            Amount = newPayment,
                            TransactionType = "In",
                            AccountId = entity.PaymentTypeId,
                            LedgerId = entity.LedgerId,
                            Remarks = entity.Remarks,
                            EntryBy = entity.EntryBy,
                            EntryDate = entity.EntryDate ?? DateTime.Now,
                            IsActive = true
                        };

                        _dbContext.Set<CashBankLedger>().Add(cashBankLedger);
                        _dbContext.SaveChanges();

                        // Commit transaction
                        transaction.Commit();

                        // Optionally: log success
                        Console.WriteLine($"Payment submitted successfully. CustomerPayment ID: {createdCustomerPaymentId}");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Transaction rolled back: {ex.Message}");
                        throw; // bubble up so controller returns 500
                    }
                }

                return createdCustomerPaymentId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SubmitPayment: {ex.Message}");
                throw;
            }
        }


        public int SubmitCustomerDuePayment(PaymentPerams entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            int createdCustomerPaymentId = 0;

            try
            {
                // Ensure PaymentDate is valid
                DateTime paymentDate;
                if (!DateTime.TryParse(entity.PaymentDate, out paymentDate))
                {
                    paymentDate = entity.EntryDate ?? DateTime.Now;
                }

                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        // 1) Find invoice
                        Invoice invoice = null;
                        if (entity.InvoiceId.HasValue && entity.InvoiceId.Value > 0)
                        {
                            invoice = _dbContext.Set<Invoice>().FirstOrDefault(i => i.Id == entity.InvoiceId.Value);
                        }
                        else if (!string.IsNullOrEmpty(entity.InvoiceNo))
                        {
                            invoice = _dbContext.Set<Invoice>().FirstOrDefault(i => i.InvoiceNo == entity.InvoiceNo);
                        }

                        if (invoice == null)
                            throw new Exception($"Invoice not found. InvoiceNo: {entity.InvoiceNo}, InvoiceId: {entity.InvoiceId}");

                        // 2) Update invoice paid amount
                        decimal currentPaid = invoice.PaidAmount ?? 0m;
                        decimal newPayment = entity.PaidAmount ?? 0m;
                        invoice.PaidAmount = currentPaid + newPayment;
                        invoice.UpdateBy = entity.EntryBy;
                        invoice.UpdateDate = entity.EntryDate ?? DateTime.Now;

                        _dbContext.Set<Invoice>().Update(invoice);
                        _dbContext.SaveChanges();

                        // 3) Insert into POStblCustomerPayment
                        var customerPayment = new CustomerPayment
                        {
                            CustomerMobile = invoice.CustomerNumber,
                            InvoiceId = invoice.Id,
                            PaymentDate = paymentDate,
                            PaidAmount = newPayment,
                            AccountId = entity.PaymentTypeId,
                            EntryBy = entity.EntryBy,
                            EntryDate = entity.EntryDate ?? DateTime.Now,
                            UpdateBy = null,
                            UpdateDate = null,
                            IsActive = true
                        };

                        _dbContext.Set<CustomerPayment>().Add(customerPayment);
                        _dbContext.SaveChanges();
                
                        transaction.Commit();

                        Console.WriteLine($"Payment submitted successfully. CustomerPayment ID: {createdCustomerPaymentId}");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Transaction rolled back: {ex.Message}");
                        throw;
                    }
                }

                return createdCustomerPaymentId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SubmitPayment: {ex.Message}");
                throw;
            }
        }

        public int SubmitCustomerDuePaymentNew(PaymentPerams entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            int createdCustomerPaymentId = 0;

            try
            {
                // Ensure PaymentDate is valid
                DateTime paymentDate;
                if (!DateTime.TryParse(entity.PaymentDate, out paymentDate))
                {
                    paymentDate = entity.EntryDate ?? DateTime.Now;
                }

                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        // 1) Find invoice
                        Invoice invoice = null;
                        if (entity.InvoiceId.HasValue && entity.InvoiceId.Value > 0)
                        {
                            invoice = _dbContext.Set<Invoice>().FirstOrDefault(i => i.Id == entity.InvoiceId.Value);
                        }
                        else if (!string.IsNullOrEmpty(entity.InvoiceNo))
                        {
                            invoice = _dbContext.Set<Invoice>().FirstOrDefault(i => i.InvoiceNo == entity.InvoiceNo);
                        }

                        if (invoice == null)
                            throw new Exception($"Invoice not found. InvoiceNo: {entity.InvoiceNo}, InvoiceId: {entity.InvoiceId}");

                        // 2) Update invoice paid amount
                        decimal currentPaid = invoice.PaidAmount ?? 0m;
                        decimal newPayment = entity.PaidAmount ?? 0m;
                        invoice.PaidAmount = currentPaid + newPayment;
                        invoice.UpdateBy = entity.EntryBy;
                        invoice.UpdateDate = entity.EntryDate ?? DateTime.Now;

                        _dbContext.Set<Invoice>().Update(invoice);
                        _dbContext.SaveChanges();

                        // 3) Insert into POStblCustomerPayment
                        var customerPayment = new CustomerPayment
                        {
                            CustomerMobile = invoice.CustomerNumber,
                            InvoiceId = invoice.Id,
                            PaymentDate = paymentDate,
                            PaidAmount = newPayment,
                            AccountId = entity.PaymentTypeId,
                            EntryBy = entity.EntryBy,
                            EntryDate = entity.EntryDate ?? DateTime.Now,
                            UpdateBy = null,
                            UpdateDate = null,
                            IsActive = true
                        };

                        _dbContext.Set<CustomerPayment>().Add(customerPayment);
                        _dbContext.SaveChanges();
                        createdCustomerPaymentId = customerPayment.Id;

                        // 4) Insert into POStblCashBankLedger
                        var cashBankLedger = new CashBankLedger
                        {
                            TranDate = paymentDate,
                            SourceId = customerPayment.Id,
                            SourceType = "Invoice",
                            LedgerType = "Income",
                            Amount = newPayment,
                            TransactionType = "In",
                            AccountId = entity.PaymentTypeId,
                            LedgerId = entity.LedgerId,
                            EntryBy = entity.EntryBy,
                            EntryDate = entity.EntryDate ?? DateTime.Now,
                            IsActive = true
                        };

                        _dbContext.Set<CashBankLedger>().Add(cashBankLedger);
                        _dbContext.SaveChanges();

                        transaction.Commit();

                        Console.WriteLine($"Payment submitted successfully. CustomerPayment ID: {createdCustomerPaymentId}");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Transaction rolled back: {ex.Message}");
                        throw;
                    }
                }

                return createdCustomerPaymentId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SubmitPayment: {ex.Message}");
                throw;
            }
        }

        //public string UpdateCustomerPhoneNo(POStblCustomer customer)
        //{
        //    try
        //    {
        //        string responseMsg = "";

        //        if (customer != null && customer.Id > 0)
        //        {
        //            // Get the existing customer
        //            var existingCustomer = _ICommonService.GetAll<POStblCustomer>()
        //                                        .FirstOrDefault(x => x.Id == customer.Id);

        //            if (existingCustomer != null)
        //            {
        //                // Update fields
        //                existingCustomer.Name = customer.Name;
        //                existingCustomer.Address = customer.Address;
        //                existingCustomer.PhoneNo = customer.PhoneNo;
        //                existingCustomer.DOB = customer.DOB;
        //                existingCustomer.Cmail = customer.Cmail;
        //                existingCustomer.SsmNubmer = customer.SsmNubmer;
        //                existingCustomer.TaxNumber = customer.TaxNumber;
        //                existingCustomer.UpdateBy = customer.UpdateBy;
        //                existingCustomer.UpdateDate = DateTime.Now;
        //                existingCustomer.AccountNo = customer.AccountNo;   
        //                existingCustomer.TIN = customer.TIN;  
        //                existingCustomer.BRN = customer.BRN;

        //                _ICommonService.Update<POStblCustomer>(existingCustomer); 

        //                responseMsg = "Customer Updated Successfully";
        //            }
        //            else
        //            {
        //                responseMsg = "Customer not found";
        //            }
        //        }
        //        else
        //        {
        //            responseMsg = "Invalid customer data";
        //        }

        //        return responseMsg;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        public string UpdateCustomerPhoneNo(POStblCustomer customer)
        {
            if (customer == null || customer.Id <= 0)
                return "Invalid customer data";

            var existingCustomer = _ICommonService
                .GetAll<POStblCustomer>()
                .FirstOrDefault(x => x.Id == customer.Id);

            if (existingCustomer == null)
                return "Customer not found";

            // Update customer fields
            existingCustomer.Name = customer.Name;
            existingCustomer.Address = customer.Address;
            existingCustomer.PhoneNo = customer.PhoneNo;
            existingCustomer.DOB = customer.DOB;
            existingCustomer.Cmail = customer.Cmail;
            existingCustomer.SsmNubmer = customer.SsmNubmer;   
            existingCustomer.TaxNumber = customer.TaxNumber;
            existingCustomer.AccountNo = customer.AccountNo;
            existingCustomer.TIN = customer.TIN;
            existingCustomer.BRN = customer.BRN;
            existingCustomer.CusCode = customer.CusCode;
            existingCustomer.UpdateBy = customer.UpdateBy;
            existingCustomer.UpdateDate = DateTime.Now;

            _ICommonService.Update(existingCustomer);

            // Only try to handle ledger if CusCode is provided
            if (!string.IsNullOrWhiteSpace(customer.CusCode))
            {
                var customerGroupLedger = _ICommonService
                    .GetAll<AccILedger>()
                    .FirstOrDefault(x => x.Title == "Customer");

                if (customerGroupLedger == null)
                    return "Customer group ledger not found";

                var existingLedger = _ICommonService
                    .GetAll<AccILedger>()
                    .FirstOrDefault(x => x.ParentId == customerGroupLedger.Id && x.Code == customer.CusCode);

                if (existingLedger == null)
                {
                    //  new ledger
                    var newLedger = new AccILedger
                    {
                        CompCode = customerGroupLedger.CompCode,
                        GLedgerId = customerGroupLedger.GLedgerId,
                        Code = customer.CusCode,
                        Title = customer.Name,
                        ParentId = customerGroupLedger.Id,
                        OPBalance = 0,
                        PaymentType = "Cash",
                        EntryBy = customer.UpdateBy,
                        EntryDate = DateTime.Now,
                        IsActive = true,
                       
                    };

                    _ICommonService.Add(newLedger);
                    //Console.WriteLine($"Ledger created for code: {customer.CusCode}");
                   
                }
                else
                {
                    //  existing ledger
                    existingLedger.Title = customer.Name ?? existingLedger.Title;
                    existingLedger.UpdateBy = customer.UpdateBy;
                    existingLedger.UpdateDate = DateTime.Now;

                    // If your service tracks changes → you may need:
                     _ICommonService.Update(existingLedger);
                    // or no call at all if change tracking is automatic
                }
            }

            // If everything went well
            return "Customer Updated Successfully";
        }
    }
}
