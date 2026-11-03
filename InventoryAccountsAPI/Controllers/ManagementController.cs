
using InventoryAccountsAPI.BusinessLayer.Interface;
using InventoryAccountsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FTSoftAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly ICategory _Category;
        private readonly IBankAccount _BankAccount;
        private readonly ISalesMan _SalasMan;
        public ManagementController(ICategory Categorys, IBankAccount BankAccount, ISalesMan salasMan)
        {
            _Category = Categorys;
            _BankAccount = BankAccount;
            _SalasMan = salasMan;
        }

        [Route("AddCategory")]
        [HttpPost]
        public int Post([FromBody] POStblCategory entity)
        {
            _Category.Add(entity);
            return entity.Id;
        }

        [Route("GetAllCategory")]
        [HttpGet]
        public List<POStblCategory> GetAllCategory()
        {
            try
            {
                List<POStblCategory> categoryList = new List<POStblCategory>();

                categoryList = _Category.GetAll();
                return categoryList;
            }
            catch
            {
                throw;
            }
        }

        [Route("GetCategorybyId")]
        [HttpGet]
        public async Task<IActionResult> GetCategorybyId(int Id)
        {
            try
            {
                var category = await _Category.GetById(Id);

                if (category != null)
                {
                    // Return a 200 OK response with the retrieved account
                    return Ok(category);
                }
                else
                {
                    // Return a 404 Not Found response if the account is not found
                    return NotFound($"Account with ID {Id} not found");
                }
            }

            catch (Exception ex)
            {
                // Handle other exceptions and return a 500 Internal Server Error response
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Route("UpdateCategory")]
        [HttpPut]
        public int Put([FromBody] POStblCategory entity)
        {
            _Category.Update(entity);
            return entity.Id;
        }

        [Route("DeleteCategory")]
        [HttpPost]
        public int Remove([FromBody] POStblCategory entity)
        {
            _Category.Remove(entity);
            return entity.Id;
        }



        [Route("AddAccount")]
        [HttpPost]
        public int Post([FromBody] POStblAccount entity)
        {
            return _BankAccount.Add(entity);
        }

        // Get All Accounts
        [Route("GetAllAccount")]
        [HttpGet]
        public List<POStblAccount> GetAllAccount()
        {
            try
            {
                return _BankAccount.GetAll();
            }
            catch
            {
                throw;
            }
        }

        // Get Account by ID
        [Route("GetAccountById")]
        [HttpGet]
        public async Task<IActionResult> GetAccountById(int id)
        {
            try
            {
                var account = await _BankAccount.GetById(id);

                if (account != null)
                    return Ok(account);
                else
                    return NotFound($"Account with ID {id} not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Update Account
        [Route("UpdateAccount")]
        [HttpPut]
        public int Put([FromBody] POStblAccount entity)
        {
            return _BankAccount.Update(entity);
        }

        // Delete Account
        [Route("DeleteAccount")]
        [HttpPost] // Using POST to delete as in your Category example
        public int Remove([FromBody] POStblAccount entity)
        {
            return _BankAccount.Remove(entity);
        }

        [HttpPost("ToggleAccountStatus")]
        public async Task<IActionResult> ToggleAccountStatus(int id, bool isActive)
        {
            var result = await _BankAccount.ToggleStatus(id, isActive);
            if (!result) return NotFound();

            return Ok(new { success = true });
        }


        #region SalesMan

        [Route("AddSalesMan")]
        [HttpPost]
        public int Post([FromBody] POStblSalesMan entity)
        {
            return _SalasMan.Add(entity);
        }

        // Get All Accounts
        [Route("GetAllSalesMan")]
        [HttpGet]
        public List<POStblSalesMan> GetAllSalesMan()
        {
            try
            {
                return _SalasMan.GetAll();
            }
            catch
            {
                throw;
            }
        }

        // Get Account by ID
        [Route("GetSalesManById")]
        [HttpGet]
        public async Task<IActionResult> GetSalesManById(int id)
        {
            try
            {
                var salesMan = await _SalasMan.GetById(id);

                if (salesMan != null)
                    return Ok(salesMan);
                else
                    return NotFound($"SalesMan with ID {id} not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Update Account
        [Route("UpdateSalesMan")]
        [HttpPut]
        public int Put([FromBody] POStblSalesMan entity)
        {
            return _SalasMan.Update(entity);
        }

        // Delete Account
        [Route("DeleteSalesMan")]
        [HttpPost] // Using POST to delete as in your Category example
        public int Remove([FromBody] POStblSalesMan entity)
        {
            return _SalasMan.Remove(entity);
        }

        #endregion
    }

}
