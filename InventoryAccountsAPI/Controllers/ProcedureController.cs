//using Dapper;
//using FTSoftAPI.Service.BusinessLayer.Interface;
//using FTSoftAPI.Service.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;


//namespace NNGTyreAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProcedureController : ControllerBase
//    {       
//        private readonly IProcedure _procedure;
//        private readonly FTDbContext _context;

//        public ProcedureController(IProcedure IProcedure, FTDbContext context)
//        {
//            _procedure = IProcedure;
//            _context = context;
//        }


//        [HttpPost]
//        [Route("executeProcedure")]
//        public async Task<IActionResult> ExecuteProcedure([FromBody] ProcedureExecuteRequest request)
//        {          
//            try
//            {
//                var affectedRows = await _procedure.ExecuteProcedureNonQueryAsync(request);
//                return Ok(new { AffectedRows = affectedRows });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest("Invalid operation type. Allowed: READ, INSERT, UPDATE, DELETE.");
//            }          
//        }

//        [HttpPost]
//        [Route("GetData")]
//        public async Task<IActionResult> GetData([FromBody] ProcedureExecuteRequest request)
//        {
//            try
//            {
//                var data = await _procedure.CallProcedureAsync1(request);
//                return Ok(data);
//            }
//            catch(Exception ex)
//            {
//                return BadRequest("Invalid operation type. Allowed: READ, INSERT, UPDATE, DELETE.");
//            }

//            // Call different logic based on operation type
//            //switch (request.Operation.ToUpper())
//            //{
//            //    case "READ":
//            //        var data = await _procedure.CallProcedureAsync(request.ProcedureName, parameters);
//            //        return Ok(data);

//            //    case "INSERT":
//            //    case "UPDATE":
//            //    case "DELETE":
//            //        var affectedRows = await _procedure.ExecuteProcedureNonQueryAsync(request.ProcedureName, parameters);
//            //        return Ok(new { AffectedRows = affectedRows });

//            //    default:
//            //        return BadRequest("Invalid operation type. Allowed: READ, INSERT, UPDATE, DELETE.");
//            //}
//        }
//    }
//}
