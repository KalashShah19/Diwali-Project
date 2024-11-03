using Npgsql;
using WebAPI.Libraries;
using WebAPI.Models;

namespace WebAPI.Repositories;

public class SchoolInfoRepository : ISchoolInfoRepository
{
    private readonly NpgsqlConnection connection;
    public SchoolInfoRepository(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString)) throw new Exception("PostgreSQL connection string is not defined.");
        NpgsqlDataSource dataSource = NpgsqlDataSource.Create(connectionString);
        this.connection = dataSource.OpenConnection();
    }

    public SchoolInfo.Get GetSchoolInfo()
    {
        NpgsqlCommand getSchoolInfoCommand = new("SELECT * FROM t_school_info", connection);
        using NpgsqlDataReader reader = getSchoolInfoCommand.ExecuteReader();
        if (reader.Read()) return new() { SchoolAddress = reader.GetString(0), SchoolContactNumber = reader.GetString(1), PrincipalImage = reader.GetString(2), PrincipalName = reader.GetString(3), PrincipalQualification = reader.GetString(4) };
        throw new Exception("Failed to fetch school information.");
    }

    public void UpdateSchoolInfo(SchoolInfo.Post schoolInfo)
    {
        SchoolInfo.Get currentSchoolInfo = GetSchoolInfo();
        NpgsqlCommand updateSchoolInfoCommand = new("UPDATE t_school_info SET c_school_address = @address, c_school_contact_number = @contactnumber, c_principal_image = @principalimage, c_principal_name = @principalname, c_principal_qualification = @principalqualification", connection);
        updateSchoolInfoCommand.Parameters.AddWithValue("address", schoolInfo.SchoolAddress);
        updateSchoolInfoCommand.Parameters.AddWithValue("contactnumber", schoolInfo.SchoolContactNumber);
        updateSchoolInfoCommand.Parameters.AddWithValue("principalimage", (schoolInfo.PrincipalImage != null) ? FileHandler.UpdatePrincipalProfilePicture(schoolInfo.PrincipalImage) : currentSchoolInfo.PrincipalImage);
        updateSchoolInfoCommand.Parameters.AddWithValue("principalname", schoolInfo.PrincipalName);
        updateSchoolInfoCommand.Parameters.AddWithValue("principalqualification", schoolInfo.PrincipalQualification);
        updateSchoolInfoCommand.ExecuteNonQuery();
    }
}