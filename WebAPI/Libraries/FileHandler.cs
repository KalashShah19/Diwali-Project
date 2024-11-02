namespace WebAPI.Libraries
{
    public static class FileHandler
    {
        private const string RootPath = "wwwroot";
        private const string SchoolDataPath = $"{RootPath}/SchoolData";
        private const string StudentDataPath = $"{RootPath}/StudentData";
        private const string TeacherDataPath = $"{RootPath}/TeacherData";
        public static string UpdatePrincipalProfilePicture(IFormFile file)
        {
            if (file.Length <= 0 || file == null) throw new NullReferenceException(nameof(file));
            string fileName = $"{SchoolDataPath}/principal{Path.GetExtension(file.FileName)}";
            using FileStream fileStream = new(fileName, FileMode.Create);
            file.CopyTo(fileStream);
            return fileName;
        }
    }
}