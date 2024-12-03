using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using WebAPI.Models;
using WebAPI.Repositories.Interfaces;

namespace WebAPI.Repositories
{
    public class CaptchaRepository : ICaptchaRepository
    {

        private readonly NpgsqlConnection connection;

        public CaptchaRepository(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString)) throw new Exception("PostgreSQL connection string is not defined.");
            NpgsqlDataSource dataSource = NpgsqlDataSource.Create(connectionString);
            this.connection = dataSource.OpenConnection();
        }

        public void GenerateCaptchaImage(string path, string captchaText)
        {
            try
            {
                // Use System.Drawing to generate the captcha image
                using (var bitmap = new Bitmap(200, 80)) // Adjust dimensions as needed
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    // Set the background color to white
                    graphics.Clear(Color.White);

                    // Create a random pattern of dots for a dotted background
                    Random rand = new Random();
                    for (int i = 0; i < 100; i++) // Adjust number of dots as needed
                    {
                        // Randomly position the dots
                        int x = rand.Next(0, 200);
                        int y = rand.Next(0, 50);
                        int radius = rand.Next(2, 5); // Random radius for the dots

                        // Draw the dot (a small ellipse)
                        graphics.FillEllipse(Brushes.LightGray, x, y, radius, radius);
                    }

                    // Set the font (unique font like "Comic Sans MS" with size 30, bold)
                    using (var font = new Font("Comic Sans MS", 30, FontStyle.Bold))
                    {
                        // Set the text color to dark blue
                        graphics.DrawString(captchaText, font, Brushes.DarkBlue, new PointF(10, 10)); // Draw the text on the image
                    }

                    // Save the image as PNG
                    bitmap.Save(path, ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                // Handle any errors (logging, etc.)
                Console.WriteLine("Error generating captcha image: " + ex.Message);
            }
        }

        public string GenerateRandomCaptchaText()
        {
            // You can generate a random string of characters for the captcha text
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuwxyz0123456789";
            Random random = new Random();
                // Console.WriteLine(random);
            return new string(Enumerable.Range(0, 6) // Length of the captcha text
                                        .Select(_ => chars[random.Next(chars.Length)])
                                        .ToArray());
        }

        public (CaptchaRequest,string) GetCaptchaModel()
        {
            string CaptchaPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "captcha");

            var model = new CaptchaRequest();
            Random rnd = new Random();

            // Ensure the directory exists
            if (!Directory.Exists(CaptchaPath))
            {
                Directory.CreateDirectory(CaptchaPath);
            }

            var files = Directory.GetFiles(CaptchaPath).ToList();
               

            // If files exist, remove the existing captcha files
            if (files.Count > 0)
            {
                // Get the random captcha ID for the existing file
                var existingCaptchaID = Path.GetFileNameWithoutExtension(files[rnd.Next(files.Count)]);

                // Define paths for image and text file
                string existingCaptchaImagePath = Path.Combine(CaptchaPath, existingCaptchaID + ".png");
                string existingCaptchaTextPath = Path.Combine(CaptchaPath, existingCaptchaID + ".txt");

                // Remove the existing captcha image and text file
                if (File.Exists(existingCaptchaImagePath))
                {
                    File.Delete(existingCaptchaImagePath);  // Delete the image
                }

                if (File.Exists(existingCaptchaTextPath))
                {
                    File.Delete(existingCaptchaTextPath);  // Delete the text file
                }
            }

            // Generate a new captcha ID and text
            string newCaptchaID = Guid.NewGuid().ToString();  // Renamed the variable
            string captchaText = this.GenerateRandomCaptchaText();

            // Generate captcha image
            string captchaImagePath = Path.Combine(CaptchaPath, newCaptchaID + ".png");
            GenerateCaptchaImage(captchaImagePath, captchaText);

            // Save captcha text to a .txt file
            string captchaTextPath = Path.Combine(CaptchaPath, newCaptchaID + ".txt");
            System.IO.File.WriteAllText(captchaTextPath, captchaText);

            // Update the model with the new captcha ID and image path
            model.CaptchaID = newCaptchaID;  // Use newCaptchaID here
            model.Captcha = Path.Combine("\\captcha", newCaptchaID + ".png");

            // Clear the old captcha text from session and store the new one
            // httpContextAccessor.HttpContext!.Session.SetString("captcha", captchaText);

            return (model,captchaText);
        }


        // public bool ValidateCaptcha(string captchaValue)
        // {


        //     if (string.IsNullOrEmpty(captchaValue))
        //     {
        //         return false;
        //     }


        //     return captchaValue == "expected_value";
        // }



    }
}