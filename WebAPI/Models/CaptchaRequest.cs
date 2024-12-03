using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class CaptchaRequest
    {
        public string? Value { get; set; }
        public string? CaptchaID { get; set; }
        public string? Captcha { get; set; }

    }
}