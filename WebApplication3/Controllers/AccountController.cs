using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using WebApplication3.Data;
using WebApplication3.Models;
using System;
using WebApplication3.ViewModels;
using Microsoft.Identity.Client;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Text;
using System.Globalization;
namespace WebApplication3.Controllers
{
    public class AccountController : Controller
    {


        private readonly ApplicationDbContext _context;
        private readonly string accountSid = "AC006beaa84e35ff6c239e1ab6eb38e155";
        private readonly string authToken = "3a8fbd6c62afd1cec4e3638347361e13";
        private readonly string twilioPhoneNumber = "+16465808947"; // Twilio phone number


        private readonly string _infobipApiKey = "feb1f237f083f04968dd83bd9a0a44b9-c1360cb7-e98b-4a0c-8e25-2af7bc57a041";
        private readonly string _infobipBaseUrl = "https://api.infobip.com/sms/2/text/single";
        private readonly string _infobipPhoneNumber = "+639060525801";

        [HttpPost]
        public async Task<IActionResult> SendOtp(string phoneNumber)
        {
            try
            {
                // Generate a random OTP
                Random random = new Random();
                string otp = random.Next(100000, 999999).ToString();

               
                var contactNumber = HttpContext.Session.GetString("ContactNumber");

                ViewBag.ContactNumber = contactNumber;
                phoneNumber = ViewBag.ContactNumber;

                string messageBody = $"Your OTP for transaction is: {otp}";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"App {_infobipApiKey}");

                    var content = new StringContent(
                        $"{{\"from\":\"{_infobipPhoneNumber}\",\"to\":\"{phoneNumber}\",\"text\":\"{messageBody}\"}}",
                        Encoding.UTF8,
                        "application/json"
                    );

                    var response = await client.PostAsync(_infobipBaseUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        HttpContext.Session.SetString("OTP", otp);
                        TempData["OtpSent"] = "OTP sent successfully!";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["OtpError"] = $"Error sending OTP: {response.StatusCode}";
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["OtpError"] = $"Error sending OTP: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [Route("Account/VerifyOtp")]
        public IActionResult VerifyOtp(string otp, string pin, string senderAccountNumber, string recipientAccountNumber, decimal amountToSend, string transactionType, string description)
        {
            try
            {
                if (HttpContext.Session.GetString("OTP") == otp)
                {
                    HttpContext.Session.Remove("OTP");

                    var sender = _context.Users.FirstOrDefault(u => u.AccountNumber == senderAccountNumber);
                    var recipient = _context.Users.FirstOrDefault(u => u.AccountNumber == recipientAccountNumber);

                    if (sender == null)
                    {
                        TempData["SendMoneyError"] = "Sender account not found.";
                        return RedirectToAction("Index", "Home");
                    }
                    if (recipient == null)
                    {
                        TempData["SendMoneyError"] = "Recipient account not found.";
                        return RedirectToAction("Index", "Home");
                    }
                    if (sender.Pin != pin)
                    {
                        TempData["OtpVerificationError"] = "Invalid PIN. Please try again.";
                        return RedirectToAction("Index", "Home");
                    }
                    if (sender.Balance >= amountToSend)
                    {
                        sender.Balance -= amountToSend;
                        recipient.Balance += amountToSend;

                        var senderTransaction = new Transaction
                        {
                            AccountName = sender.Username,
                            AccountNumber = sender.AccountNumber,
                            Amount = -amountToSend,
                            TransactionType = "Send",
                            TransactionDate = DateTime.Now,
                            Description = description
                        };

                        var recipientTransaction = new Transaction
                        {
                            AccountName = recipient.Username,
                            AccountNumber = recipient.AccountNumber,
                            Amount = amountToSend,
                            TransactionType = "Receive",
                            TransactionDate = DateTime.Now,
                            Description = description
                        };

                        _context.Transactions.Add(senderTransaction);
                        _context.Transactions.Add(recipientTransaction);
                        _context.SaveChanges();

                        // Format the amount to Philippine Pesos
                        var cultureInfo = new System.Globalization.CultureInfo("fil-PH");
                        TempData["SendMoneySuccess"] = $"Successfully sent {amountToSend.ToString("C", cultureInfo)} to {recipientAccountNumber}.";
                        HttpContext.Session.Remove("OtpVerified");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["SendMoneyError"] = "Insufficient balance to send money.";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["OtpVerificationError"] = "Invalid OTP. Please try again.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["OtpVerificationError"] = "Error verifying OTP: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        public async Task<IActionResult> SendOtpWithdrawal(decimal withdrawAmount)
        {
            try
            {
                Random random = new Random();
                string otp = random.Next(100000, 999999).ToString();

                var contactNumber = HttpContext.Session.GetString("ContactNumber");

         
                string messageBody = $"Your OTP for withdrawal of {withdrawAmount.ToString("C", new CultureInfo("fil-PH"))} is: {otp}";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"App {_infobipApiKey}");

                    var content = new StringContent(
                        $"{{\"from\":\"{_infobipPhoneNumber}\",\"to\":\"{contactNumber}\",\"text\":\"{messageBody}\"}}",
                        Encoding.UTF8,
                        "application/json"
                    );

                    var response = await client.PostAsync(_infobipBaseUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        HttpContext.Session.SetString("OTPWithdrawal", otp);
                        return Ok(new { message = "OTP sent successfully for withdrawal" });
                    }
                    else
                    {
                        return BadRequest("Error sending OTP for withdrawal");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error sending OTP for withdrawal: {ex.Message}");
            }
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Inbox()


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

            string accountNumber = HttpContext.Session.GetString("AccountNumber");

           
            var transactions = _context.Transactions
                                        .Where(t => t.AccountNumber == accountNumber || t.RecipientAccountNumber == accountNumber)
                                        .Select(t => new Transaction
                                        {
                                            TransactionId = t.TransactionId,
                                            AccountName = t.AccountName ?? "N/A", 
                                            AccountNumber = t.AccountNumber,
                                            TransactionType = t.TransactionType,
                                            Amount = t.Amount,
                                            TransactionDate = t.TransactionDate,
                                            Description = t.Description,
                                            RecipientAccountNumber = t.RecipientAccountNumber ?? "N/A", 
                                            RequestStatus = t.RequestStatus ?? "N/A" 
                                        })
                                        .ToList();

            return View(transactions);
        }






        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Username does not exist";
                }
                else if (user.Password != model.Password)
                {
                    TempData["ErrorMessage"] = "Incorrect password";
                }
                else
                {
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("AccountNumber", user.AccountNumber);
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
        }



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

        [HttpPost]
        public JsonResult CheckContactNumber(string contactNumber)
        {
            bool isRegistered = _context.Users.Any(u => u.ContactNumber == contactNumber);
            return Json(new { isRegistered });
        }

        [HttpPost]
        public JsonResult CheckAccountNumber(string accountNumber)
        {
            bool isRegistered = _context.Users.Any(u => u.AccountNumber == accountNumber);
            return Json(new { isRegistered });
        }

        [HttpGet]
        public IActionResult ViewAccount()
        {

            return View();
        }

        [HttpGet]
        public IActionResult Transaction()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SendOtp1(string phoneNumber)
        {
            try
            {
                TwilioClient.Init(accountSid, authToken);


                Random random = new Random();
                string otp = random.Next(100000, 999999).ToString();


                string messageBody = $"Your OTP for transaction is: {otp}";


                var message = MessageResource.Create(
                    body: messageBody,
                    from: new PhoneNumber(twilioPhoneNumber),
                    to: new PhoneNumber(phoneNumber)
                );

                HttpContext.Session.SetString("OTP", otp);


                TempData["OtpSent"] = "OTP sent successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {

                TempData["OtpError"] = $"Error sending OTP: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult SendMoney(string senderAccountNumber, string recipientAccountNumber, decimal amountToSend, string transactionType, string description)
        {
            if (HttpContext.Session.GetString("OtpVerified") != "true")
            {
                TempData["SendMoneyError"] = "OTP verification required.";
                return RedirectToAction("Index", "Home");
            }

            string senderUserId = HttpContext.Session.GetString("UserId");

            var sender = _context.Users.FirstOrDefault(u => u.AccountNumber == senderAccountNumber);
            var recipient = _context.Users.FirstOrDefault(u => u.AccountNumber == recipientAccountNumber);

            if (sender == null || recipient == null)
            {
                TempData["SendMoneyError"] = "Sender or recipient account not found.";
                return RedirectToAction("Index", "Account");
            }

            if (sender.Balance >= amountToSend)
            {
                sender.Balance -= amountToSend;
                recipient.Balance += amountToSend;

                var senderTransaction = new Transaction
                {
                    AccountName = sender.Username,
                    AccountNumber = sender.AccountNumber,
                    Amount = -amountToSend,
                    TransactionType = "Send",
                    TransactionDate = DateTime.Now,
                    Description = description
                };

                var recipientTransaction = new Transaction
                {
                    AccountName = recipient.Username,
                    AccountNumber = recipient.AccountNumber,
                    Amount = amountToSend,
                    TransactionType = "Receive",
                    TransactionDate = DateTime.Now,
                    Description = description
                };

                _context.Transactions.Add(senderTransaction);
                _context.Transactions.Add(recipientTransaction);
                _context.SaveChanges();

                // Adjust the success message to replace $ with ₱
                string amountToSendFormatted = $"₱{amountToSend:N2}";

                TempData["SendMoneySuccess"] = $"Successfully sent {amountToSendFormatted} to {recipientAccountNumber}.";

                HttpContext.Session.Remove("OtpVerified");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["SendMoneyError"] = "Insufficient balance to send money.";
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        public IActionResult Withdraw(decimal withdrawAmount, string otp, string pin)
        {
            try
            {
                // Validate OTP
                if (HttpContext.Session.GetString("OTPWithdrawal") != otp)
                {
                    TempData["WithdrawError"] = "Invalid OTP. Please try again.";
                    return RedirectToAction("Index", "Home");
                }

            
                HttpContext.Session.Remove("OTPWithdrawal");

               
                string userId = HttpContext.Session.GetString("UserId");
                var user = _context.Users.FirstOrDefault(u => u.Id.ToString() == userId);

                if (user == null)
                {
                    TempData["WithdrawError"] = "User not found.";
                    return RedirectToAction("Index", "Home");
                }

                if (user.Pin != pin)
                {
                    TempData["WithdrawError"] = "Invalid PIN. Please try again.";
                    return RedirectToAction("Index", "Home");
                }

                if (withdrawAmount > 0 && user.Balance >= withdrawAmount)
                {
                    user.Balance -= withdrawAmount;

                    var transaction = new Transaction
                    {
                        AccountName = user.Username,
                        AccountNumber = user.AccountNumber,
                        Amount = -withdrawAmount,
                        TransactionType = "Withdrawal",
                        TransactionDate = DateTime.Now,
                        Description = "Withdrawal"
                    };

                    _context.Transactions.Add(transaction);
                    _context.SaveChanges();

                
                    var cultureInfo = new System.Globalization.CultureInfo("fil-PH");
                    TempData["WithdrawalSuccess"] = $"Successfully withdrew {withdrawAmount.ToString("C", cultureInfo)}.";

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["WithdrawError"] = "Withdrawal failed. Please check your balance or try again later.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["WithdrawError"] = $"Error processing withdrawal: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        public IActionResult AddDeposit(DepositViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
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

                    user.Balance += model.Amount;

                    var transaction = new Transaction
                    {
                        AccountName = user.Username,
                        AccountNumber = user.AccountNumber,
                        Amount = model.Amount,
                        TransactionType = "Deposit",
                        TransactionDate = DateTime.Now,
                        Description = "Deposit"
                    };

                    _context.Users.Update(user);
                    _context.Transactions.Add(transaction);
                    _context.SaveChanges();

                    // Format the amount to Philippine Pesos
                    var cultureInfo = new System.Globalization.CultureInfo("fil-PH");
                    TempData["DepositSuccess"] = $"Successfully deposited {model.Amount.ToString("C", cultureInfo)}.";
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    TempData["DepositError"] = $"Error depositing: {ex.Message}";
                    if (ex.InnerException != null)
                    {
                        TempData["DepositError"] += $" Inner exception: {ex.InnerException.Message}";
                    }
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
        }


        [HttpPost]
        public IActionResult AddTransaction(TransactionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var transaction = new Transaction
                {
                    AccountName = model.AccountName,
                    AccountNumber = model.AccountNumber,
                    Amount = model.Amount,
                    TransactionType = model.TransactionType,
                    TransactionDate = DateTime.Now,
                    Description = model.Description
                };

                _context.Transactions.Add(transaction);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            TempData["LogoutMessage"] = "You have been logged out successfully.";

            return RedirectToAction("Login", "Account");
        }


        [HttpPost]
        public IActionResult SubmitRequestMoney(RequestMoneyViewModel model)
        {
            try
            {
                string senderAccountNumber = HttpContext.Session.GetString("AccountNumber");
                var sender = _context.Users.FirstOrDefault(u => u.AccountNumber == senderAccountNumber);

                if (sender == null)
                {
                    TempData["RequestMoneyError"] = "Sender account not found.";
                    return RedirectToAction("Index", "Home");
                }


                var requestTransaction = new Transaction
                {
                    AccountName = sender.Username,
                    AccountNumber = sender.AccountNumber,
                    RecipientAccountNumber = model.RecipientAccountNumber,
                    Amount = model.Amount,
                    TransactionType = "Request",
                    RequestStatus = "Pending", 
                    TransactionDate = DateTime.Now,
                    Description = model.Description
                };

                _context.Transactions.Add(requestTransaction);
                _context.SaveChanges();

                TempData["RequestMoneySuccess"] = "Money request submitted successfully.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["RequestMoneyError"] = $"Error submitting money request: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult ProcessRequestMoney(int transactionId, string action)
        {
            try
            {
                var transaction = _context.Transactions.FirstOrDefault(t => t.TransactionId == transactionId);

                if (transaction == null)
                {
                    TempData["RequestMoneyError"] = "Transaction not found.";
                    return RedirectToAction("Index", "Account");
                }

                if (action == "accept")
                {
                
                    var recipient = _context.Users.FirstOrDefault(u => u.AccountNumber == transaction.RecipientAccountNumber);

                    if (recipient == null)
                    {
                        TempData["RequestMoneyError"] = "Recipient account not found.";
                        return RedirectToAction("MoneyRequests", "Account");
                    }

                   
                    if (recipient.Balance < transaction.Amount)
                    {
                        TempData["RequestMoneyError"] = "Insufficient balance to approve this request.";
                        return RedirectToAction("MoneyRequests", "Account");
                    }

               
                    var sender = _context.Users.FirstOrDefault(u => u.AccountNumber == transaction.AccountNumber);

                    if (sender == null)
                    {
                        TempData["RequestMoneyError"] = "Sender account not found.";
                        return RedirectToAction("MoneyRequests", "Account");
                    }

                   
                    sender.Balance += transaction.Amount;
                    recipient.Balance  -= transaction.Amount;

                   
                    transaction.RequestStatus = "Accepted";
                    transaction.TransactionDate = DateTime.Now; 

                    _context.SaveChanges();

                    TempData["RequestMoneySuccess"] = "Request accepted successfully.";
                }
                else if (action == "reject")
                {
                    // Process rejection logic here
                    transaction.RequestStatus = "Rejected";
                    // Update other necessary fields in the transaction record
                    transaction.TransactionDate = DateTime.Now; // Example update if needed

                    _context.SaveChanges();

                    TempData["RequestMoneySuccess"] = "Request rejected successfully.";
                }

                return RedirectToAction("MoneyRequests"); // Redirect to MoneyRequests action
            }
            catch (Exception ex)
            {
                TempData["RequestMoneyError"] = $"Error processing request: {ex.Message}";
                return RedirectToAction("MoneyRequests", "Account");
            }
        }




        [HttpGet]

        public IActionResult ViewMoneyRequests()
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


            string accountNumber = HttpContext.Session.GetString("AccountNumber");

            var moneyRequests = _context.Transactions
                                        .Where(t => t.AccountNumber == accountNumber && t.TransactionType == "Request")
                                        .ToList();

            return View(moneyRequests);
        }


        [HttpGet]
        public IActionResult MoneyRequests()
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

          
            string accountNumber = HttpContext.Session.GetString("AccountNumber");

          
            var moneyRequests = _context.Transactions
                                       .Where(t => t.RecipientAccountNumber == accountNumber && t.TransactionType == "Request")
                                       .ToList();

            return View(moneyRequests); 
        }


    }
}

