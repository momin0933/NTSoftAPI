using BMSAPI.BusinessLayer.Interface;
using BMSAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        private readonly IBuilding _Service;

        public BuildingController(IBuilding building)
        {
            _Service = building;
        }

        [Route("Add")]
        [HttpPost]
        public int Add([FromBody] Building entity)
        {
            return _Service.Add(entity);
        }

        [Route("Update")]
        [HttpPut]
        public int Update([FromBody] Building entity)
        {
            return _Service.Update(entity);
        }

        [Route("Remove")]
        [HttpDelete]
        public int Remove([FromBody] Building entity)
        {
            return _Service.Remove(entity);
        }

        [Route("AddExpenseBill")]
        [HttpPost]
        public int AddExpenseBill([FromBody] Expenses entity)
        {
            return _Service.AddExpenseBill(entity);
        }

        [Route("GetBuildingWiseFlat")]
        [HttpGet]
        public IActionResult GetBuildingWiseFlat()
        {
            try
            {
                return Ok(_Service.GetAll());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString()); // সাময়িক: debug শেষে সরিয়ে দিন
            }
        }

        [Route("GetDashboardData")]
        [HttpGet]
        public List<RptDashboard> GetDashboardData()
        {
            return _Service.GetDashboardData();
        }

        [Route("GetAllExpensesList")]
        [HttpGet]
        public List<RptExpense> GetAllExpenses(DateTime fromDate, DateTime toDate)
        {
            return _Service.GetAllExpenses(fromDate, toDate);
        }

        [Route("GetAllBuildingFlatAndOwner")]
        [HttpGet]
        public List<Building> GetAllBuildingFlatOwner()
        {
            return _Service.GetAllBuildingFlatOwner();
        }

        [Route("AddFlats")]
        [HttpPost]
        public int AddFlats([FromBody] Flats entity)
        {
            return _Service.AddFlat(entity);
        }

        [Route("UpdateFlats")]
        [HttpPut]
        public int UpdateFlats([FromBody] Flats entity)
        {
            return _Service.UpdateFlat(entity);
        }

        [Route("RemoveFlats")]
        [HttpDelete]
        public int RemoveFlats([FromBody] Flats entity)
        {
            return _Service.RemoveFlat(entity);
        }

        [Route("GetExpenseById")]
        [HttpGet]
        public List<RptExpense> GetExpenseById(int id)
        {
            return _Service.GetExpenseById(id);
        }

        [HttpPost("Delete")]
        public IActionResult DeleteExpnse([FromBody] Expenses expenses)
        {
            _Service.DeleteExpense(expenses);
            return Ok(new { message = "Deleted item" });
        }
    }
}