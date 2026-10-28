using BMSAPI.BusinessLayer.Interface;
using BMSAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessage _Service;
        public MessageController(IMessage message)
        {
            _Service = message;
        }
        [Route("Add")]
        [HttpPost]
        public int Add([FromBody] SendMessage entity)
        {
            _Service.Add(entity);
            return entity.Id;
        }
    }
}
