using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WebAPI.Libraries
{
    public class Mailer
    {
        private readonly NetworkCredential credential;

        public Mailer()
        {
            string emailaddress = "ashishvadhaviya05@gmail.com";
            string emailpassword = "pssv tcad kvbd yrhu";
            credential = new(emailaddress, emailpassword);
        }

        public string? SendMail(string recipient, string message)
        {
            try
            {
                MailAddress fromAddress = new(credential.UserName, "OTP");
                MailAddress toAddress = new(recipient);
                const string subject = "OTP";

                SmtpClient smtp =
                    new()
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = credential,
                    };
                using MailMessage mailMessage =
                    new(fromAddress, toAddress)
                    {
                        IsBodyHtml = true,
                        Subject = subject,
                        Body = message,
                    };
                smtp.Send(mailMessage);
                return "true";
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return e.Message;
            }

            return "";
        }

        // public static string PasswordChangeMail(string emailAddress, string otp)
        // {
        //     return $"<h3>Password Reset OTP</h3><p>Your OTP for resetting your password is: <strong>{otp}</strong></p><p>Please use this OTP within the next 10 minutes to reset your password.</p>";
        // }
       public static string PasswordChangeMail(string emailAddress, string otp)
{
    return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset OTP</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f3f4f6;
            color: #555;
            margin: 0;
            padding: 0;
        }}

        .email-container {{
            width: 100%;
            max-width: 600px;
            margin: 30px auto;
            padding: 20px;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
        }}

        .email-header {{
            text-align: center;
            background-color: #28a745;
            color: white;
            padding: 20px;
            border-radius: 8px 8px 0 0;
        }}

        .email-header h2 {{
            margin: 0;
            font-size: 26px;
        }}

        .email-body {{
            margin-top: 20px;
            line-height: 1.6;
            font-size: 16px;
        }}

        .otp {{
            font-size: 30px;
            font-weight: bold;
            color: #007BFF;
            margin: 20px 0;
            text-align: center;
            letter-spacing: 2px;
        }}

        .otp strong {{
            font-size: 32px;
            color: #333;
        }}

        .button {{
            background-color: #28a745;
            color: white;
            padding: 12px 24px;
            text-decoration: none;
            border-radius: 6px;
            display: inline-block;
            margin-top: 30px;
            font-size: 16px;
            width: 100%;
            box-sizing: border-box;
            text-align: center;
            transition: background-color 0.3s ease;
        }}

        .button:hover {{
            background-color: #218838;
            cursor: pointer;
        }}

        .footer {{
            text-align: center;
            margin-top: 30px;
            font-size: 14px;
            color: #888;
        }}

        .footer p {{
            margin: 5px 0;
        }}

        .footer a {{
            color: #28a745;
            text-decoration: none;
            transition: text-decoration 0.3s ease;
        }}

        .footer a:hover {{
            text-decoration: underline;
        }}

        .email-body p {{
            color: #333;
            font-size: 16px;
            line-height: 1.5;
        }}

        .email-body p a {{
            color: #007BFF;
            text-decoration: none;
        }}

        .email-body p a:hover {{
            text-decoration: underline;
        }}

        /* Mobile responsiveness */
        @media (max-width: 600px) {{
            .email-container {{
                width: 100%;
                padding: 15px;
            }}
            .email-header h2 {{
                font-size: 22px;
            }}
            .button {{
                font-size: 14px;
                padding: 10px 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='email-header'>
            <h2>Password Reset Request</h2>
        </div>
        <div class='email-body'>
            <p>Hello there,</p>
            <p>We received a request to reset your password. If you made this request, please use the following OTP:</p>
            <p class='otp'><strong>{otp}</strong></p>
            
        </div>
       
        <div class='footer'>          
            <p>Thank you for being with us!</p>
        </div>
    </div>
</body>
</html>";
}

    }
}
