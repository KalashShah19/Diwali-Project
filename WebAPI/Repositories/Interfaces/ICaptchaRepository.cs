using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Repositories.Interfaces
{
    public interface ICaptchaRepository
    {
        public (CaptchaRequest,string) GetCaptchaModel();
        public string GenerateRandomCaptchaText();
        public void GenerateCaptchaImage(string path, string captchaText);
    }

}