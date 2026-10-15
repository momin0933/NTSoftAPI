using BMSAPI.BusinessLayer.Interface;
using BMSAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommitteeSmsMailController : ControllerBase
    {
        private readonly ICommitteeSmsMail _service;
        private readonly IWebHostEnvironment _env;

        public CommitteeSmsMailController(ICommitteeSmsMail service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        public class CreateCommitteeSmsMailRequest
        {
            public string? MailSub { get; set; }
            public string? Mail { get; set; }
            public string? SMS { get; set; }
            public List<int> MemberIds { get; set; } = new List<int>();
            public string? EntryBy { get; set; }
            public string? Remarks { get; set; }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create()
        {
            string? savedFilePath = null; // DB সেভ fail হলে file মুছে ফেলার জন্য

            try
            {
                string? mailSub = null;
                string? mail = null;
                string? sms = null;
                List<int> memberIds = new();
                string? entryBy = null;
                string? remarks = null;
                string? mailAttachPath = null;

                if (Request.HasFormContentType)
                {
                    var form = await Request.ReadFormAsync();

                    mailSub = form["MailSub"].ToString();
                    mail = form["Mail"].ToString();
                    sms = form["SMS"].ToString();
                    remarks = form["Remarks"].ToString();
                    entryBy = form["EntryBy"].ToString();

                    var memberIdsJson = form["MemberIds"].ToString();
                    if (string.IsNullOrEmpty(memberIdsJson) || !TryParseMemberIds(memberIdsJson, out memberIds))
                        return BadRequest(new { Result = "Failed", ErrorMessage = "Invalid MemberIds" });

                    var file = form.Files["attachment"];
                    if (file != null && file.Length > 0)
                    {
                        if (file.Length > 10 * 1024 * 1024) // 10MB
                            return BadRequest(new { Result = "Failed", ErrorMessage = "File too large (max 10MB)" });

                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".txt" };
                        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(ext))
                            return BadRequest(new { Result = "Failed", ErrorMessage = $"File type {ext} not allowed" });

                        var smsAppName = form["SMSAppName"].ToString();
                        if (string.IsNullOrWhiteSpace(smsAppName))
                            return BadRequest(new { Result = "Failed", ErrorMessage = "SMSAppName is required for folder structure" });

                        var invalidChars = Path.GetInvalidFileNameChars();
                        var safeFolderName = string.Join("_", smsAppName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
                        if (string.IsNullOrEmpty(safeFolderName))
                            safeFolderName = "Unknown";

                        // wwwroot/uploads/committee-attachments/{SMSAppName}/
                        var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
                        var dynamicFolder = Path.Combine(webRoot, "uploads", "committee-attachments", safeFolderName);
                        Directory.CreateDirectory(dynamicFolder);

                        var safeFileName = Path.GetFileNameWithoutExtension(Path.GetFileName(file.FileName));
                        safeFileName = string.Join("_", safeFileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
                        if (string.IsNullOrEmpty(safeFileName))
                            safeFileName = "attachment";

                        var finalFileName = $"{safeFileName}{ext}";
                        var fullPath = Path.Combine(dynamicFolder, finalFileName);

                        var counter = 1;
                        while (System.IO.File.Exists(fullPath))
                        {
                            finalFileName = $"{safeFileName}_{counter}{ext}";
                            fullPath = Path.Combine(dynamicFolder, finalFileName);
                            counter++;
                        }

                        await using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        savedFilePath = fullPath;
                        mailAttachPath = $"/uploads/committee-attachments/{safeFolderName}/{finalFileName}";
                    }
                }
                else // JSON (file ছাড়া)
                {
                    var request = await Request.ReadFromJsonAsync<CreateCommitteeSmsMailRequest>();
                    if (request == null)
                        return BadRequest(new { Result = "Failed", ErrorMessage = "Invalid body" });

                    mailSub = request.MailSub;
                    mail = request.Mail;
                    sms = request.SMS;
                    remarks = request.Remarks;
                    entryBy = request.EntryBy ?? "System";
                    memberIds = request.MemberIds ?? new();
                }

                if (memberIds.Count == 0)
                    return BadRequest(new { Result = "Failed", ErrorMessage = "No members selected" });

                var defaultUser = string.IsNullOrWhiteSpace(entryBy) ? "System" : entryBy.Trim();

                var committeeSmsMail = new CommitteeSmSMail
                {
                    MailSub = mailSub ?? "",
                    Mail = mail ?? "",
                    SMS = sms ?? "",
                    MailAttatchPath = mailAttachPath,
                    Remarks = remarks ?? "",
                    EntryBy = defaultUser,
                    UpdateBy = defaultUser
                };

                var members = memberIds.Select(memberId => new SmsMailWithMember
                {
                    MemberId = memberId,
                    EntryBy = defaultUser,
                    UpdateBy = defaultUser
                }).ToList();

                var id = await _service.CreateCommitteeSmsMail(committeeSmsMail, members);

                return Ok(new
                {
                    Result = "Success",
                    Id = id,
                    Message = "Communication record saved successfully",
                    AttachmentPath = mailAttachPath
                });
            }
            catch (Exception ex)
            {
                // DB সেভ না হলে আপলোড করা file রেখে লাভ নেই
                if (savedFilePath != null && System.IO.File.Exists(savedFilePath))
                {
                    try { System.IO.File.Delete(savedFilePath); } catch { }
                }

                var inner = ex.InnerException?.Message ?? "None";
                Console.WriteLine($"SAVE ERROR: {ex.Message} | Inner: {inner}");
                return StatusCode(500, new
                {
                    Result = "Failed",
                    ErrorMessage = "Save failed",
                    Detail = inner
                });
            }
        }

        private static bool TryParseMemberIds(string json, out List<int> ids)
        {
            ids = new();
            try
            {
                var list = System.Text.Json.JsonSerializer.Deserialize<List<int>>(json);
                if (list != null) ids = list;
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _service.GetById(id);
                if (result == null)
                    return NotFound(new { Result = "Failed", ErrorMessage = "Record not found" });

                return Ok(new { Result = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Result = "Failed", ErrorMessage = ex.Message });
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAll();
                return Ok(new { Result = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Result = "Failed", ErrorMessage = ex.Message });
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CommitteeSmSMail request)
        {
            try
            {
                request.Id = id;
                var success = await _service.Update(request);

                if (!success)
                    return NotFound(new { Result = "Failed", ErrorMessage = "Record not found" });

                return Ok(new { Result = "Success", Message = "Record updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Result = "Failed", ErrorMessage = ex.Message });
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.Delete(id);

                if (!success)
                    return NotFound(new { Result = "Failed", ErrorMessage = "Record not found" });

                return Ok(new { Result = "Success", Message = "Record deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Result = "Failed", ErrorMessage = ex.Message });
            }
        }
    }
}