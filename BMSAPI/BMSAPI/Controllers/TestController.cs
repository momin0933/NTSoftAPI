using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]   
        public IActionResult Get()
        {
            return Ok(new { success = true, message = "API is working!" });
        }
    }
}
