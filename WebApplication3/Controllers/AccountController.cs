using Microsoft.AspNetCore.Mvc;
using WebApplication3.Data;
using WebApplication3.Models;
namespace WebApplication3.Controllers
{
    public class AccountController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly string accountSid = "AC006beaa84e35ff6c239e1ab6eb38e155";
        private readonly string authToken = "3a8fbd6c62afd1cec4e3638347361e13";
        private readonly string twilioPhoneNumber = "+16465808947"; // Twilio phone number


        private readonly string _infobipApiKey = "212952af9d98fc971b0aa3735e4d5684-9d867386-32dd-4a29-81f7-1bc5f1dbb225";
        private readonly string _infobipBaseUrl = "https://api.infobip.com/sms/2/text/single";
        private readonly string _infobipPhoneNumber = "+639203614524";

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("", "Username already exists");
                    return View(model);
                }

                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("", "Email already exists");
                    return View(model);
                }

                if (_context.Users.Any(u => u.ContactNumber == model.ContactNumber))
                {
                    ModelState.AddModelError("", "Contact number already exists");
                    return View(model);
                }

                if (_context.Users.Any(u => u.AccountNumber == model.AccountNumber))
                {
                    ModelState.AddModelError("", "Account number already exists");
                    return View(model);
                }

                if (model.Password != model.RePassword)
                {
                    ModelState.AddModelError("", "Passwords do not match");
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    Name = model.Name,
                    DateOfBirth = model.DateOfBirth,
                    ContactNumber = model.ContactNumber,
                    Email = model.Email,
                    Address = model.Address,
                    Branch = model.Branch,
                    AccountType = model.AccountType,
                    AccountNumber = model.AccountNumber,
                    Pin = model.Pin,
                    SecurityQuestion1 = model.SecurityQuestion1,
                    Answer1 = model.Answer1,
                    SecurityQuestion2 = model.SecurityQuestion2,
                    Answer2 = model.Answer2
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}