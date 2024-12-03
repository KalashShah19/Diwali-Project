namespace WebAPI.Libraries
{
    public static class FileHandler
    {
        private const string UserPath = $"UserData";
        private const string RootPath = "wwwroot";
        private const string SchoolDataPath = $"SchoolData";
        private const string StudentDataPath = $"StudentData";
        private const string TeacherDataPath = $"TeacherData";
        public static string UpdatePrincipalProfilePicture(IFormFile file)
        {
            if (file.Length <= 0 || file == null) throw new NullReferenceException(nameof(file));
            string fileName = $"{SchoolDataPath}/principal{Path.GetExtension(file.FileName)}";
            using FileStream fileStream = new($"{RootPath}/{fileName}", FileMode.Create);
            file.CopyTo(fileStream);
            return fileName;
        }

        public static string UserProfilePicture(IFormFile file, string userId)
        {
            try
            {
                if (file.Length <= 0 || file == null) throw new NullReferenceException(nameof(file));
                string fileName = $"{UserPath}/{userId}{Path.GetExtension(file.FileName)}";
                Console.WriteLine($"File Name: {fileName}");
                using (FileStream fileStream = new($"{RootPath}/{fileName}", FileMode.Create))
                    file.CopyTo(fileStream);
                return fileName;
            }
            catch (Exception ex)
            {

                return $"An error occurred: {ex.Message}";
            }
        }
    }
}