using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;


namespace WebAPI.Repositories
{
    public class MaterialRepository
    {
        public readonly NpgsqlConnection connection;
        public MaterialRepository(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString)) throw new Exception("PostgreSQL connection string is not defined.");
            NpgsqlDataSource dataSource = NpgsqlDataSource.Create(connectionString);
            this.connection = dataSource.OpenConnection();
        }




        public int GetTeacherId(int userId)
        {
            // var conn = _materialService.connection;
            var tId = new NpgsqlCommand($"select c_teacher_id from t_teachers where c_user_id = {userId}", connection).ExecuteScalar();
            return Convert.ToInt32(tId);
        }
        public bool hasAccess(int tId, int cwsid)
        {
            // var conn = _materialService.connection;
            var accessCommand = new NpgsqlCommand("select count(*) from t_classwise_subjects where c_id = @cwsid and c_teacher_id = @tid", connection);
            accessCommand.Parameters.AddWithValue("cwsid", cwsid);
            accessCommand.Parameters.AddWithValue("tid", tId);
            var count = Convert.ToInt32(accessCommand.ExecuteScalar());
            return count > 0;
        }
        public async Task<bool> UploadMaterial(int cwsid, IFormFile file, int tId)
        {

            var fileName = Guid.NewGuid().ToString() + file.FileName;
            var _rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf", fileName);
            using (var stream = new FileStream(_rootPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var cpath = "/pdf/" + fileName;
            var uploadCommand = new NpgsqlCommand("INSERT INTO public.t_material(c_materialname, c_cwsid, c_uploadtime, c_file, c_materialtype, c_teacher_id) VALUES (@mname, @cwsid, CURRENT_TIMESTAMP, @path, @type, @tid)", connection);
            uploadCommand.Parameters.AddWithValue("mname", file.FileName);
            uploadCommand.Parameters.AddWithValue("cwsid", cwsid);
            uploadCommand.Parameters.AddWithValue("path", cpath);
            uploadCommand.Parameters.AddWithValue("type", "Material");
            uploadCommand.Parameters.AddWithValue("tid", tId);
            var count = uploadCommand.ExecuteNonQuery();
            return count > 0;
        }
        public bool DeleteMaterial(string path, int tId, int global_cwsid)
        {
            var selectMaterialCmd = new NpgsqlCommand("delete from  t_material where c_cwsid=@cwsid and c_teacher_id =@tid and c_file = @file", connection);
            selectMaterialCmd.Parameters.AddWithValue("cwsid", global_cwsid);
            selectMaterialCmd.Parameters.AddWithValue("tid", tId);
            selectMaterialCmd.Parameters.AddWithValue("file", path);
            var result = selectMaterialCmd.ExecuteNonQuery();
            return result > 1;            
        }
    }
}