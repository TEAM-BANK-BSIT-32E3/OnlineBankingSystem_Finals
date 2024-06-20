using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebApplication3.Data;
using WebApplication3.Models;
using Newtonsoft.Json;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _infobipApiKey = "feb1f237f083f04968dd83bd9a0a44b9-c1360cb7-e98b-4a0c-8e25-2af7bc57a041";
        private readonly string _infobipBaseUrl = "https://api.infobip.com/sms/2/text/single";
        private readonly string _infobipPhoneNumber = "+639060525801";

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Settings()
        {   

            string userIdString = HttpContext.Session.GetString("UserId");

            if (userIdString == null || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Username = user.Username;
            ViewBag.AccountNumber = user.AccountNumber;
            ViewBag.Balance = user.Balance;
            ViewBag.ContactNumber = user.ContactNumber;
            ViewBag.ContactVerified = user.Contact_verified;

            HttpContext.Session.SetString("ContactNumber", user.ContactNumber);

            return View();
        }


        public IActionResult Index()
        {
            string userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Username = user.Username;
            ViewBag.AccountNumber = user.AccountNumber;
            ViewBag.Balance = user.Balance;
            ViewBag.ContactNumber = user.ContactNumber;
            ViewBag.ContactVerified = user.Contact_verified; 
            HttpContext.Session.SetString("ContactNumber", user.ContactNumber);
            HttpContext.Session.SetString("AccountNumber", user.AccountNumber);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendOtp(string contactNumber)
        {
            string userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string otpGenerationTimeString = HttpContext.Session.GetString("OtpGenerationTime");
            DateTime? otpGenerationTime = string.IsNullOrEmpty(otpGenerationTimeString) ? (DateTime?)null : DateTime.Parse(otpGenerationTimeString);

            if (otpGenerationTime.HasValue && (DateTime.Now - otpGenerationTime.Value).TotalSeconds < 60)
            {
             
                ViewBag.OtpSent = false;
                ViewBag.ErrorMessage = "Please wait 60 seconds before resending OTP.";
                ViewBag.ContactNumber = contactNumber;
                ViewBag.RemainingTime = (int)(60 - (DateTime.Now - otpGenerationTime.Value).TotalSeconds);
                return View("Settings");
            }

            string otp = GenerateOtp();
            HttpContext.Session.SetString("Otp", otp);
            HttpContext.Session.SetString("OtpGenerationTime", DateTime.Now.ToString());

            string message = $"Your OTP code is {otp}";

           
            string toNumber = ValidateAndFormatPhoneNumber(contactNumber) ?? user.ContactNumber;

            var payload = new
            {
                from = "YourAppName",
                to = toNumber,
                text = message
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "App " + _infobipApiKey);
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_infobipBaseUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    ViewBag.OtpSent = true;
                   
                    HttpContext.Session.SetString("NewContactNumber", contactNumber);
                }
                else
                {
                    ViewBag.OtpSent = false;
                }
            }

            ViewBag.ContactNumber = contactNumber;
            ViewBag.RemainingTime = 60;
            return View("Settings");
        }

        [HttpPost]
        public async Task<IActionResult> SendOtpWithdrawal(decimal withdrawAmount)
        {
            try
            {
                Random random = new Random();
                string otp = random.Next(100000, 999999).ToString();

                var contactNumber = HttpContext.Session.GetString("ContactNumber");

                string messageBody = $"Your OTP for withdrawal of {withdrawAmount:C} is: {otp}";

            

                HttpContext.Session.SetString("OTPWithdrawal", otp);

                return Ok(new { message = "OTP sent successfully for withdrawal" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error sending OTP for withdrawal: {ex.Message}");
            }
        }




        [HttpPost]
        public IActionResult VerifyOtp(string enteredOtp)
        {
            string sessionOtp = HttpContext.Session.GetString("Otp");
            if (sessionOtp == enteredOtp)
            {
              
                string userIdString = HttpContext.Session.GetString("UserId");
                if (userIdString != null && int.TryParse(userIdString, out int userId))
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                    if (user != null)
                    {
                       
                        string newContactNumber = HttpContext.Session.GetString("NewContactNumber");
                        user.ContactNumber = newContactNumber;

                      
                        user.Contact_verified = true;

                      
                        _context.SaveChanges();

                       
                        HttpContext.Session.SetString("ContactNumber", newContactNumber);

                       
                        user = _context.Users.FirstOrDefault(u => u.Id == userId);
                        ViewBag.ContactVerified = user.Contact_verified; 
                    }
                }

                ViewBag.OtpVerified = true;
            }
            else
            {
                ViewBag.OtpVerified = false;
            }

            ViewBag.ContactNumber = HttpContext.Session.GetString("ContactNumber");

      
            HttpContext.Session.Remove("OtpGenerationTime");
            ViewBag.RemainingTime = 0;

            return View("Settings");
        }


        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string ValidateAndFormatPhoneNumber(string inputNumber)
        {
           
            string pattern = @"^\+639\d{9}$";

            if (Regex.IsMatch(inputNumber, pattern))
            {
                return inputNumber;
            }

            return null;
        }

        [HttpPost]
        public IActionResult UpdatePassword(string currentPassword, string newPassword, string confirmNewPassword)
        {
            string userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (user.Password != currentPassword)
            {
                ViewBag.PasswordUpdateMessage = "Current password is incorrect. Password not updated.";
                return View("Settings");
            }

            if (newPassword != confirmNewPassword)
            {
                ViewBag.PasswordUpdateMessage = "New password and confirm password do not match. Password not updated.";
                return View("Settings");
            }


            user.Password = newPassword;
            _context.SaveChanges();

        
            ViewBag.PasswordUpdateMessage = "Password updated successfully!";

            
            ViewBag.ContactVerified = user.Contact_verified;

            HttpContext.Session.SetString("ContactNumber", user.ContactNumber);

            return View("Settings");
        }

        [HttpPost]
        public IActionResult UpdatePin(string currentPin, string newPin, string confirmNewPin)
        {
            string userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (user.Pin != currentPin)
            {
                ViewBag.PinUpdateMessage = "Current PIN is incorrect. PIN not updated.";
                return View("Settings");
            }

            if (newPin != confirmNewPin)
            {
                ViewBag.PinUpdateMessage = "New PIN and confirm PIN do not match. PIN not updated.";
                return View("Settings");
            }

 
            user.Pin = newPin;
            _context.SaveChanges();

          
            ViewBag.PinUpdateMessage = "PIN updated successfully!";
            ViewBag.ContactVerified = user.Contact_verified; 

            HttpContext.Session.SetString("ContactNumber", user.ContactNumber);
            return View("Settings");
        }



    }
}
