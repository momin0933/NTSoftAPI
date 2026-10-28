using BMSAPI.BusinessLayer.Interface;
using BMSAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers.Accounts
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccGLController : ControllerBase
    {
        private readonly IAccounts _accounts;

        public AccGLController(IAccounts Accounts)
        {
            _accounts = Accounts;
        }

        [Route("GetAll")]
        [HttpGet]
        public List<AccGLedger> GetAll()
        {
            try
            {
                List<AccGLedger> accountList = new List<AccGLedger>();

                accountList = _accounts.GetAll();
                return accountList;
            }
            catch
            {
                throw;
            }
        }



        [Route("Add")]
        [HttpPost]
        public int Post([FromBody] AccGLedger entity)
        {
            _accounts.Add(entity);
            return entity.Id;
        }


        [Route("Update")]
        [HttpPut]
        public int Put([FromBody] AccGLedger entity)
        {
            _accounts.Update(entity);
            return entity.Id;
        }

        [Route("DeleteAccount")]
        [HttpDelete]
        public int Remove([FromBody] AccGLedger entity)
        {
            _accounts.Remove(entity);
            return entity.Id;
        }



        [Route("GetById")]
        [HttpGet]
        public async Task<IActionResult> GetbyId(int Id)
        {
            try
            {
                var account = await _accounts.GetById(Id);

                if (account != null)
                {
                    // Return a 200 OK response with the retrieved account
                    return Ok(account);
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

    }
}
