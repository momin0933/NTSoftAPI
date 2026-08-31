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
                p.Add("@Batch", model.Batch);
                p.Add("@RollSection", model.RollSection);
                p.Add("@Division", model.Division);
                p.Add("@Category", model.Category);
                p.Add("@Guest", model.Guest);
                p.Add("@PresentAddress", model.PresentAddress);
                p.Add("@PremanetAddress", model.PremanetAddress);
                p.Add("@Occupation", model.Occupation);
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

      

    }
}