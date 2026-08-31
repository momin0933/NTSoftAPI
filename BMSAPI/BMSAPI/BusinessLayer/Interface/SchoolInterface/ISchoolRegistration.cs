using BMSAPI.Models.School;
namespace BMSAPI.BusinessLayer.Interface.SchoolInterface
{
    public interface ISchoolRegistration
    {
        RegistrationResult RegisterStudent(SchoolUserRegistration model);

        IEnumerable<SchoolUserRegistration> GetRegistrationReport(
           DateTime? fromDate,
           DateTime? toDate
       );

        // Registration By Id
        SchoolUserRegistration GetRegistrationById(int id);

        RegistrationResult UpdateRegistration(SchoolUserRegistration model);
    }
}
