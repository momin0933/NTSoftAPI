
using BMSAPI.BusinessLayer.Interface.SchoolInterface;
using BMSAPI.Models.School;
using Microsoft.AspNetCore.Mvc;

namespace BMSAPI.Controllers.AppControllers.ProHUBControllers
{
    [ApiController]
    public class SchoolRegistrationController : ControllerBase
    {
        private readonly ISchoolRegistration _schoolRegistrationService;
        private readonly ILogger<SchoolRegistrationController> _logger;

        public SchoolRegistrationController(
            ISchoolRegistration schoolRegistrationService,
            ILogger<SchoolRegistrationController> logger)
        {
            _schoolRegistrationService =
                schoolRegistrationService
                ?? throw new ArgumentNullException(
                    nameof(schoolRegistrationService));

            _logger =
                logger
                ?? throw new ArgumentNullException(
                    nameof(logger));
        }


        // =========================================================
        // REGISTER STUDENT
        // POST: api/RegisterStudent
        // =========================================================

        [HttpPost("api/RegisterStudent")]
        public IActionResult RegisterStudent(
            [FromBody] SchoolUserRegistration model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Registration data is required"
                    });
                }


                if (
                    string.IsNullOrWhiteSpace(model.Name) ||
                    string.IsNullOrWhiteSpace(model.Phone) ||
                    string.IsNullOrWhiteSpace(model.Category)
                )
                {
                    return BadRequest(new
                    {
                        success = false,
                        message =
                            "Name, Phone, and Category are required"
                    });
                }


                var result =
                    _schoolRegistrationService
                        .RegisterStudent(model);


                if (!result.Success)
                {
                    return StatusCode(
                        500,
                        new
                        {
                            success = false,
                            message =
                                "Registration failed, please try again"
                        });
                }


                return Ok(new
                {
                    success = true,
                    registrationId = model.RegNo,
                    id = result.Id,
                    message = "Registration সফল হয়েছে"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error registering student with Phone: {Phone}",
                    model?.Phone);

                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message = ex.Message
                    });
            }
        }


        // =========================================================
        // REGISTRATION REPORT
        // QueryChecker = 1
        //
        // GET:
        // api/RegistrationReport
        //
        // Example:
        // api/RegistrationReport?fromDate=2026-08-01&toDate=2026-08-31
        // =========================================================

        [HttpGet("api/RegistrationReport")]
        public IActionResult RegistrationReport(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var result =
                    _schoolRegistrationService
                        .GetRegistrationReport(
                            fromDate,
                            toDate);


                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error loading registration report. FromDate: {FromDate}, ToDate: {ToDate}",
                    fromDate,
                    toDate);

                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message =
                            ex.Message
                    });
            }
        }


        // =========================================================
        // REGISTRATION BY ID
        // QueryChecker = 2
        //
        // GET:
        // api/RegistrationReport/1
        // =========================================================

        [HttpGet("api/RegistrationReport/{id:int}")]
        public IActionResult RegistrationById(
            int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid registration Id"
                    });
                }


                var result =
                    _schoolRegistrationService
                        .GetRegistrationById(id);


                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message =
                            "Registration not found"
                    });
                }


                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error loading registration by Id: {Id}",
                    id);

                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message =
                            ex.Message
                    });
            }
        }


 
// =========================================================
// UPDATE REGISTRATION
// QueryChecker = 3
//
// PUT:
// api/RegistrationReport/{id}
// =========================================================

[HttpPut("api/RegistrationReport/{id:int}")]
public IActionResult UpdateRegistration(
    int id,
    [FromBody] SchoolUserRegistration model)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid registration Id"
                    });
                }

                if (model == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Registration data is required"
                    });
                }

                // URL Id must be the actual Id
                model.Id = id;

                var result =
                    _schoolRegistrationService
                        .UpdateRegistration(model);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Registration update failed"
                    });
                }

                return Ok(new
                {
                    success = true,
                    id = result.Id,
                    message = "Registration updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating registration. Id: {Id}",
                    id
                );

                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message = ex.Message
                    }
                );
            }
        }

        [HttpGet("api/Sponsors")]
        public IActionResult GetSponsors()
        {
            try
            {
                var result =
                    _schoolRegistrationService
                        .GetSponsors();

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error loading sponsors"
                );

                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message = ex.Message
                    }
                );
            }
        }

    }
}

