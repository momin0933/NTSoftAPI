using BMSAPI.Models.School;
namespace BMSAPI.BusinessLayer.Interface.SchoolInterface
{
    public interface ISchoolRegistration
    {
        RegistrationResult RegisterStudent(SchoolUserRegistration model);
    }
}
