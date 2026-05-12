using Microsoft.AspNetCore.Mvc;

namespace FureverHome.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string Email, string Password, bool RememberMe)
        {
            // TODO: Add authentication logic
            // For now, redirect to dashboard on success
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            ModelState.AddModelError("", "Invalid email or password.");
            return View();
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string FirstName, string LastName, string Email,
                                      string PhoneNumber, string Password, string ConfirmPassword)
        {
            // TODO: Add registration logic
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                return View();
            }
            return RedirectToAction("Login");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            // TODO: Add sign-out logic
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}