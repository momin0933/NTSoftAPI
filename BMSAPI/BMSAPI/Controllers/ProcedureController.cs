using BMSAPI.BusinessLayer.Interface;
using BMSAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcedureController : ControllerBase
    {
        private readonly IProcedure _procedure;

        public ProcedureController(IProcedure procedure)
        {
            _procedure = procedure;
        }

        [HttpPost]
        [Route("executeProcedure")]
        public async Task<IActionResult> ExecuteProcedure([FromBody] ProcedureExecuteRequest request)
        {
            try
            {
                var affectedRows = await _procedure.ExecuteProcedureNonQueryAsync(request);
                return Ok(new { AffectedRows = affectedRows });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        [Route("GetData")]
        public async Task<IActionResult> GetData([FromBody] ProcedureExecuteRequest request)
        {
            try
            {
                var data = await _procedure.CallProcedureAsync1(request);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}
