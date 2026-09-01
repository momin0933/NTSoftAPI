using BMSAPI.BusinessLayer.Interface.AppsInterface.ProHUB;
using BMSAPI.BusinessLayer.Interface.SchoolInterface;
using BMSAPI.BusinessLayer.Service;
using BMSAPI.Models.Apps.PropHUB;
using BMSAPI.Models.School;
using Dapper;
using Microsoft.Extensions.Logging;
using System;

namespace BMSAPI.BusinessLayer.Manager.AppManager.ProHUBManager
{
    public class SchoolRegistrationManager : ISchoolRegistration
    {
        private readonly ILogger<SchoolRegistrationManager> _logger;
        private readonly IDapperService _IDapperService;
        private const string SP_NAME = "SP_SchoolRegistration";
        private const string REPORT_SP_NAME = "SP_RegistrationReport";

        public SchoolRegistrationManager(IDapperService dapperService, ILogger<SchoolRegistrationManager> logger)
        {
            _IDapperService = dapperService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region INSERT Operation (Dapper) — QueryChecker = 1
        public RegistrationResult RegisterStudent(SchoolUserRegistration model)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@QueryChecker", 1);
                p.Add("@RegNo", model.RegNo);
                p.Add("@Name", model.Name);
                p.Add("@BName", model.BName);
                p.Add("@Phone", model.Phone);
                p.Add("@Email", model.Email);
                p.Add("@Batch", model.Batch);
                p.Add("@RollSection", model.RollSection);
                p.Add("@Division", model.Division);
                p.Add("@Category", model.Category);
                p.Add("@Guest", model.Guest);
                p.Add("@PresentAddress", model.PresentAddress);
                p.Add("@PremanetAddress", model.PremanetAddress);
                p.Add("@Occupation", model.Occupation);
                p.Add("@TotalFee", model.TotalFee);
                p.Add("@ImgPath", model.ImgPath);
                p.Add("@PaymentMethod", model.PaymentMethod);
                p.Add("@TranID", model.TranID);
                p.Add("@SpecialNote", model.SpecialNote);
                p.Add("@Status", "Pending");
                p.Add("@Remarks", model.Remarks);
                p.Add("@EntryBy", model.EntryBy);

                var result = _IDapperService.GetByDynamicSPSingle<dynamic>(SP_NAME, p);
                int affectedRows = (int)result.AffectedRows;
                int newId = (int)result.Id;

                if (affectedRows <= 0)
                {
                    _logger.LogWarning("School registration failed for Phone: {Phone}", model.Phone);
                    return new RegistrationResult { Success = false, Id = 0 };
                }

                _logger.LogInformation("Student registered successfully with Id: {Id}", newId);
                return new RegistrationResult { Success = true, Id = newId };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering student with Phone: {Phone}", model.Phone);
                throw;
            }
        }
        #endregion
        #region Registration Report — QueryChecker = 1

        public IEnumerable<SchoolUserRegistration>
            GetRegistrationReport(
                DateTime? fromDate,
                DateTime? toDate)
        {
            try
            {
                DynamicParameters p =
                    new DynamicParameters();

                p.Add("@QueryChecker", 1);

                p.Add(
                    "@FromDate",
                    fromDate?.Date
                );

                p.Add(
                    "@ToDate",
                    toDate?.Date
                );

                p.Add("@Id", null);

                var result =
                    _IDapperService
                        .GetAllBySP<SchoolUserRegistration>(
                            REPORT_SP_NAME,
                            p
                        );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error loading registration report"
                );

                throw;
            }
        }

        #endregion


        #region Registration By Id — QueryChecker = 2

        public SchoolUserRegistration
            GetRegistrationById(int id)
        {
            try
            {
                DynamicParameters p =
                    new DynamicParameters();

                p.Add("@QueryChecker", 2);
                p.Add("@FromDate", null);
                p.Add("@ToDate", null);
                p.Add("@Id", id);

                var result =
                    _IDapperService
                        .GetByDynamicSPSingle<SchoolUserRegistration>(
                            REPORT_SP_NAME,
                            p
                        );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error loading registration by Id: {Id}",
                    id
                );

                throw;
            }
        }

        #endregion

  
        #region Update Registration — QueryChecker = 3

public RegistrationResult UpdateRegistration(
    SchoolUserRegistration model)
        {
            try
            {
                if (model == null)
                {
                    return new RegistrationResult
                    {
                        Success = false,
                        Id = 0
                    };
                }

                DynamicParameters p =
                    new DynamicParameters();

                p.Add("@QueryChecker", 3);

                p.Add("@FromDate", null);
                p.Add("@ToDate", null);

                p.Add("@Id", model.Id);

                p.Add("@RegNo", model.RegNo);
                p.Add("@Name", model.Name);
                p.Add("@Email", model.Email);
                p.Add("@BName", model.BName);
                p.Add("@Phone", model.Phone);
                p.Add("@Batch", model.Batch);
                p.Add("@RollSection", model.RollSection);
                p.Add("@Division", model.Division);
                p.Add("@Category", model.Category);
                p.Add("@Guest", model.Guest);
                p.Add("@PresentAddress", model.PresentAddress);
                p.Add("@PremanetAddress", model.PremanetAddress);
                p.Add("@ImgPath", model.ImgPath);
                p.Add("@PaymentMethod", model.PaymentMethod);
                p.Add("@TranID", model.TranID);
                p.Add("@SpecialNote", model.SpecialNote);
                p.Add("@Status", model.Status);
                p.Add("@Occupation", model.Occupation);
                p.Add("@TotalFee", model.TotalFee);
                p.Add("@Remarks", model.Remarks);
                p.Add("@EntryBy", model.EntryBy);
                p.Add("@UpdateBy", model.UpdateBy);

                var result =
                    _IDapperService
                        .GetByDynamicSPSingle<dynamic>(
                            REPORT_SP_NAME,
                            p
                        );

                int affectedRows =
                    result?.AffectedRows != null
                        ? Convert.ToInt32(result.AffectedRows)
                        : 0;

                if (affectedRows <= 0)
                {
                    _logger.LogWarning(
                        "Registration update failed for Id: {Id}",
                        model.Id
                    );

                    return new RegistrationResult
                    {
                        Success = false,
                        Id = model.Id
                    };
                }

                _logger.LogInformation(
                    "Registration updated successfully. Id: {Id}",
                    model.Id
                );

                return new RegistrationResult
                {
                    Success = true,
                    Id = model.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating registration. Id: {Id}",
                    model?.Id
                );

                throw;
            }
        }

        #endregion

        #region Get Sponsors — QueryChecker = 4

        public IEnumerable<SponsorModel> GetSponsors()
        {
            try
            {
                DynamicParameters p =
                    new DynamicParameters();

                p.Add("@QueryChecker", 4);
              
                var result =
                    _IDapperService
                        .GetAllBySP<SponsorModel>(
                            REPORT_SP_NAME,
                            p
                        );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error loading sponsors"
                );

                throw;
            }
        }

        #endregion


    }
}