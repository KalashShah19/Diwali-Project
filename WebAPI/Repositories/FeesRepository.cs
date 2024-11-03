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
            feeStructure.Add(new() { FeesID = reader.GetInt16(0), Amount = reader.GetInt32(1), BatchYear = reader.GetInt16(2), Standard = reader.GetInt16(3) });
        }
        return feeStructure;
    }

    public List<FeesInfo.Get> GetFeeStructure(int batchYear)
    {
        List<FeesInfo.Get> feeStructure = [];
        NpgsqlCommand getFeeStructureCommand = new("SELECT * from t_fees_structure WHERE c_batch_year = @year", connection);
        getFeeStructureCommand.Parameters.AddWithValue("year", batchYear);
        using NpgsqlDataReader reader = getFeeStructureCommand.ExecuteReader();
        while (reader.Read())
        {
            feeStructure.Add(new() { FeesID = reader.GetInt16(0), Amount = reader.GetInt32(1), BatchYear = reader.GetInt16(2), Standard = reader.GetInt16(3) });
        }
        return feeStructure;
    }

    public FeesInfo.Get GetFeeStructure(int standard, int batchYear)
    {
        NpgsqlCommand getFeeStructureCommand = new("SELECT * from t_fees_structure WHERE c_standard = @standard, c_batch_year = @batchyear", connection);
        getFeeStructureCommand.Parameters.AddWithValue("standard", standard);
        getFeeStructureCommand.Parameters.AddWithValue("batchyear", batchYear);
        using NpgsqlDataReader reader = getFeeStructureCommand.ExecuteReader();
        if (reader.Read()) return new() { FeesID = reader.GetInt16(0), Amount = reader.GetInt32(1), BatchYear = reader.GetInt16(2), Standard = reader.GetInt16(3) };
        throw new Exception($"Failed to read fee structure of with standard {standard} of batch year {batchYear}");
    }

    public FeesInfo.Get GetFeeStructureById(int feesinfoid)
    {
        NpgsqlCommand getFeeStructureCommand = new("SELECT * from t_fees_structure WHERE c_id = @id", connection);
        getFeeStructureCommand.Parameters.AddWithValue("id", feesinfoid);
        using NpgsqlDataReader reader = getFeeStructureCommand.ExecuteReader();
        if (reader.Read()) return new() { FeesID = reader.GetInt16(0), Amount = reader.GetInt32(1), BatchYear = reader.GetInt16(2), Standard = reader.GetInt16(3) };
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