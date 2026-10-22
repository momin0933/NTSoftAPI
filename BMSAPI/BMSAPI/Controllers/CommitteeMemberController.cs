using BMSAPI.BusinessLayer.Interface;
using BMSAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommitteeMemberController : ControllerBase
    {
        private readonly ICommitteeMember _service;

        public CommitteeMemberController(ICommitteeMember service)
        {
            _service = service;
        }

        public class AddCommitteeMembersRequest
        {
            public int CId { get; set; }
            public List<CommitteeMember> Members { get; set; } = new();
            public string? EntryBy { get; set; }
        }

        [HttpPost("AddCommitteeMembers")]
        public IActionResult AddCommitteeMembers([FromBody] AddCommitteeMembersRequest request)
        {
            try
            {
                if (request?.Members == null || request.Members.Count == 0)
                {
                    return BadRequest(new
                    {
                        Result = "Failed",
                        ErrorMessage = "No members provided"
                    });
                }

                foreach (var member in request.Members)
                {
                    member.EntryBy ??= request.EntryBy ?? "System";
                    // CId, EntryDate, IsActive Manager আর SaveChanges override সেট করে
                }

                var insertedCount = _service.AddCommitteeMembers(request.CId, request.Members);

                return Ok(new
                {
                    Result = "Success",
                    InsertedCount = insertedCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Result = "Failed",
                    ErrorMessage = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
    }
}