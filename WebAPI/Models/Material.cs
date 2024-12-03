using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Material
    {
        public class UploadResponse
    {
        public bool Success { get; set; }
        public string? Path { get; set; }
        public string? FileName { get; set; }
    }

    public class FileManagerItem
    {
        public int cwsid { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public bool IsDirectory { get; set; }
        public bool HasDirectories { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
    }
}