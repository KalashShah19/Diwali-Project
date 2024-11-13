using WebAPI.Models;

namespace WebAPI.Libraries;

public static class EnumHelper
{
    public static Gender GetGender(string gender)
    {
        switch (gender)
        {
            case "Male": return Gender.Male;
            case "Female": return Gender.Female;
            default: return Gender.Others;
        }
    }

    public static UserRole GetUserRole(string userrole)
    {
        switch (userrole)
        {
            case "Teacher": return UserRole.Teacher;
            default: return UserRole.Student;
        }
    }
}