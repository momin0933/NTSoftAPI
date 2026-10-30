using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAccountsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoice _IInvoice;

        public InvoiceController(IInvoice invoice)
        {
            _IInvoice = invoice;
        }
        [Route("GetAllInvoice")]
        [HttpGet]
        public List<Invoice> GetAllInvoice() 
        {
            List<Invoice> invoiceList = _IInvoice.GetAll().Where(x=>x.IsActive == true).ToList();
            return invoiceList;
        }
        
        [Route("CreateInvoice")]
        [HttpPost]
        public int Post([FromBody] InvoicePerams entity)
        {
            try
            {
                int id = _IInvoice.AddAndUpdate(entity);
                return id;
            }
            catch (Exception ex )
            {
                Console.WriteLine(ex);
                throw ex;
            }

           
        }

        [Route("CreateInvoiceNew")]
        [HttpPost]
        public async Task<int> CreateInvoiceNew([FromBody] InvoicePerams entity)
        {
            try
            {
                int id = await _IInvoice.AddAndUpdateNew(entity);
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        [Route("UpdateInvoice")]
        [HttpPut]
        public int UpdateInvoice([FromBody] Invoice entity)
        {
            _IInvoice.Update(entity);
            return entity.Id; 
        }
        [Route("AddPhoneNo")]
        [HttpPost]
        public string AddPhoneNo(POStblCustomer customer)
        {
            string response = _IInvoice.AddCustomerPhoneNo(customer);
            return response;
        }

        [Route("UpdatePhoneNo")]
        [HttpPost]
        public string UpdatePhoneNo(POStblCustomer customer)
        {
            string response = _IInvoice.UpdateCustomerPhoneNo(customer);
            return response;
        }
        [Route("CustomerDetails")]
        [HttpGet]
        public List<POStblCustomer> CustomerDetails(string phoneNo)
        {
            var response = _IInvoice.CustomerDetails(phoneNo);
            return response;
        }
        //[Route("CustomerDetails")]
        //[HttpGet]
        //public List<POStblCustomer> CustomerDetails(string query)
        //{
        //    // Pass the query to the service method
        //    var response = _IInvoice.CustomerDetails(query);
        //    return response;
        //}

        [Route("CustomerMembershipRule")]
        [HttpGet]
        public List<CustomerMembership> CustomerMembershipRule()
        {
            var response = _IInvoice.CustomerMembershipRule();
            return response;
        }

        [Route("GetInvoiceInfoByInvoiceNo")]
        [HttpGet]
        public Invoice GetInvoiceInfoByInvoiceNo(string invoiceNo)
        {
            var response = _IInvoice.GetInvoiceInfoByInvoiceNo(invoiceNo);
            return response;
        }

        [Route("SubmitPayment")]
        [HttpPost]
        public IActionResult SubmitPayment([FromBody] PaymentPerams entity)
        {
           
            try
            {
                int id = _IInvoice.SubmitPayment(entity);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("SubmitCustomerDuePayment")]
        [HttpPost]
        public IActionResult SubmitCustomerDuePayment([FromBody] PaymentPerams entity)
        {
            try
            {
                int id = _IInvoice.SubmitCustomerDuePayment(entity);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("SubmitCustomerDuePaymentNew")]
        [HttpPost]
        public IActionResult SubmitCustomerDuePaymentNew([FromBody] PaymentPerams entity)
        {
            try
            {
                int id = _IInvoice.SubmitCustomerDuePaymentNew(entity);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
