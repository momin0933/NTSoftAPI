using InventoryAccountsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InventoryAccountsAPI.BusinessLayer.Interface
{
    public interface IInvoice : ICommonMethod<Invoice>
    {
        public int SubmitPayment(PaymentPerams entity);
        public int SubmitCustomerDuePayment(PaymentPerams entity);
        public int SubmitCustomerDuePaymentNew(PaymentPerams entity);
        public int  AddAndUpdate(InvoicePerams entity);
        public int AddAndUpdateNew(InvoicePerams entity);
        public string AddCustomerPhoneNo(POStblCustomer customer);
        public string UpdateCustomerPhoneNo(POStblCustomer customer);

        public List<POStblCustomer> CustomerDetails(string phoneNo);
        //public List<POStblCustomer> CustomerDetails(string query);
        public List<CustomerMembership> CustomerMembershipRule();

        public Invoice GetInvoiceInfoByInvoiceNo(string invoiceNo);

    }
}
