using Npgsql;
using WebAPI.Models;

namespace WebAPI.Repositories;

public class FeesRepository : IFeesRepository
{
    private readonly NpgsqlConnection connection;
    public FeesRepository(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString)) throw new Exception("PostgreSQL connection string is not defined.");
        NpgsqlDataSource dataSource = NpgsqlDataSource.Create(connectionString);
        this.connection = dataSource.OpenConnection();
    }

    public void AddFeeInfo(FeesInfo.Post feesInfo)
    {
        NpgsqlCommand addFeeStructureCommand = new("INSERT INTO t_fees_structure(c_amount, c_batch_year, c_standard) VALUES(@amount, @batchyear, @standard)", connection);
        addFeeStructureCommand.Parameters.AddWithValue("amount", feesInfo.Amount);
        addFeeStructureCommand.Parameters.AddWithValue("batchyear", feesInfo.BatchYear);
        addFeeStructureCommand.Parameters.AddWithValue("standard", feesInfo.Standard);
        addFeeStructureCommand.ExecuteNonQuery();
    }

    public List<FeesInfo.Get> GetFeeStructure()
    {
        List<FeesInfo.Get> feeStructure = [];
        NpgsqlCommand getFeeStructureCommand = new("SELECT * from t_fees_structure", connection);
        using NpgsqlDataReader reader = getFeeStructureCommand.ExecuteReader();
        while (reader.Read())
        {
            feeStructure.Add(new() { FeesID = reader.GetInt16(0), Amount = reader.GetInt32(1), BatchYear = reader.GetString(2), Standard = reader.GetString(3) });
        }
        return feeStructure;
    }


    public bool PayFees(FeesInfo.PaymentRequest paymentRequest)
    {
        const string insertPaymentQuery = @"
            INSERT INTO t_payments (c_user_id, c_id, c_paymentdate, c_status, c_currentstandard)
            VALUES (@UserId, @CId, @PaymentDate, @Status, @CurrentStandard)";

        using (var insertCmd = new NpgsqlCommand(insertPaymentQuery, connection))
        {
            insertCmd.Parameters.AddWithValue("@UserId", paymentRequest.UserId);
            insertCmd.Parameters.AddWithValue("@CId", paymentRequest.FeeStructureId);
            insertCmd.Parameters.AddWithValue("@PaymentDate", paymentRequest.PaymentDate);
            insertCmd.Parameters.AddWithValue("@Status", paymentRequest.Status);
            insertCmd.Parameters.AddWithValue("@CurrentStandard", paymentRequest.CurrentStandard);

            int rowsAffected = insertCmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }


    public List<FeesInfo.Get> GetFeeStructure(string batchYear)
    {
        List<FeesInfo.Get> feeStructure = [];
        NpgsqlCommand getFeeStructureCommand = new("SELECT * from t_fees_structure WHERE c_batch_year = @year", connection);
        getFeeStructureCommand.Parameters.AddWithValue("year", batchYear);
        using NpgsqlDataReader reader = getFeeStructureCommand.ExecuteReader();
        while (reader.Read())
        {
            feeStructure.Add(new() { FeesID = reader.GetInt16(0), Amount = reader.GetInt32(1), BatchYear = reader.GetString(2), Standard = reader.GetString(3) });
        }
        return feeStructure;
    }

    public List<FeesInfo.FeesDetailsForStudent> GetFeesDetailsStudent(int userId)
    {
        List<FeesInfo.FeesDetailsForStudent> feesDetails = new();


        NpgsqlCommand getPaymentDetailsCommand = new(
            "SELECT tp.c_paymentid, ts.c_enrollment_number, " +
            "       CASE WHEN tp.c_status = 'Pending' THEN ts.c_standard ELSE tp.c_currentstandard END AS c_currentstandard, " +
            "       tfs.c_amount, tp.c_status, tp.c_paymentdate, tfs.c_id " +
            "FROM t_payments tp " +
            "INNER JOIN t_students ts ON tp.c_user_id = ts.c_user_id " +
            "INNER JOIN t_fees_structure tfs ON tfs.c_id = tp.c_id " +
            "WHERE tp.c_user_id = @userId", connection);
        getPaymentDetailsCommand.Parameters.AddWithValue("@userId", userId);

        using (NpgsqlDataReader reader = getPaymentDetailsCommand.ExecuteReader())
        {
            while (reader.Read())
            {
                feesDetails.Add(new FeesInfo.FeesDetailsForStudent
                {
                    c_paymentid = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                    c_enrollment_number = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                    c_currentstandard = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    c_amount = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    c_status = reader.IsDBNull(4) ? "Pending" : reader.GetString(4),
                    c_paymentdate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                    c_id = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6)
                });
            }
        }

        NpgsqlCommand getStandardFeeDetailsCommand = new(
            "SELECT tp.c_paymentid, ts.c_enrollment_number, " +
            "       CASE WHEN tp.c_currentstandard IS NULL OR tp.c_currentstandard != ts.c_standard THEN ts.c_standard ELSE tp.c_currentstandard END AS c_currentstandard, " +
            "       tfs.c_amount, COALESCE(tp.c_status, 'Pending') AS c_status, tp.c_paymentdate, tfs.c_id " +
            "FROM t_students ts " +
            "LEFT JOIN t_fees_structure tfs ON ts.c_standard = tfs.c_standard " +
            "LEFT JOIN t_payments tp ON tp.c_user_id = ts.c_user_id AND tp.c_id = tfs.c_id " +
            "WHERE ts.c_user_id = @userId " +
            "AND EXTRACT('YEAR' FROM ts.c_admission_date) = CAST(tfs.c_batch_year AS int) " +
            "AND (tp.c_status = 'Pending' OR tp.c_status IS NULL);", connection);
        getStandardFeeDetailsCommand.Parameters.AddWithValue("@userId", userId);

        using (NpgsqlDataReader reader = getStandardFeeDetailsCommand.ExecuteReader())
        {
            while (reader.Read())
            {
                feesDetails.Add(new FeesInfo.FeesDetailsForStudent
                {
                    c_paymentid = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                    c_enrollment_number = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                    c_currentstandard = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    c_amount = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    c_status = reader.IsDBNull(4) ? "Pending" : reader.GetString(4),
                    c_paymentdate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                    c_id = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6)
                });
            }
        }

        return feesDetails;
    }

    public FeesInfo.Get GetFeeStructure(string standard, string batchYear)
    {
        NpgsqlCommand getFeeStructureCommand = new("SELECT * from t_fees_structure WHERE c_standard = @standard, c_batch_year = @batchyear", connection);
        getFeeStructureCommand.Parameters.AddWithValue("standard", standard);
        getFeeStructureCommand.Parameters.AddWithValue("batchyear", batchYear);
        using NpgsqlDataReader reader = getFeeStructureCommand.ExecuteReader();
        if (reader.Read()) return new() { FeesID = reader.GetInt16(0), Amount = reader.GetInt32(1), BatchYear = reader.GetString(2), Standard = reader.GetString(3) };
        throw new Exception($"Failed to read fee structure of with standard {standard} of batch year {batchYear}");
    }

    public FeesInfo.Get GetFeeStructureById(int feesinfoid)
    {
        NpgsqlCommand getFeeStructureCommand = new("SELECT * from t_fees_structure WHERE c_id = @id", connection);
        getFeeStructureCommand.Parameters.AddWithValue("id", feesinfoid);
        using NpgsqlDataReader reader = getFeeStructureCommand.ExecuteReader();
        if (reader.Read()) return new() { FeesID = reader.GetInt16(0), Amount = reader.GetInt32(1), BatchYear = reader.GetString(2), Standard = reader.GetString(3) };
        throw new Exception($"Failed to read fee structure of id {feesinfoid}");
    }

    public void RemoveFeeInfo(int feesinfoid)
    {
        NpgsqlCommand deleteFeeInfoCommand = new("DELETE FROM t_fees_structure WHERE c_id = @id", connection);
        deleteFeeInfoCommand.Parameters.AddWithValue("id", feesinfoid);
        deleteFeeInfoCommand.ExecuteNonQuery();
    }

    public void UpdateFeeInfo(FeesInfo.Get feesInfo)
    {
        NpgsqlCommand addFeeStructureCommand = new("UPDATE t_fees_structure SET c_amount = @amount, c_batch_year = @batchyear, c_standard = @standard WHERE c_id = @id", connection);
        addFeeStructureCommand.Parameters.AddWithValue("amount", feesInfo.Amount);
        addFeeStructureCommand.Parameters.AddWithValue("batchyear", feesInfo.BatchYear);
        addFeeStructureCommand.Parameters.AddWithValue("standard", feesInfo.Standard);
        addFeeStructureCommand.Parameters.AddWithValue("id", feesInfo.FeesID);
        addFeeStructureCommand.ExecuteNonQuery();
    }

}