using FureverHome.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FureverHome.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Json(new { success = false, errors = new Dictionary<string, string> { { "Email", "This email is not registered." } } });
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "Welcome back, " + user.FirstName + "!", redirectUrl = Url.Action("Index", "Home") });
                }
                
                return Json(new { success = false, errors = new Dictionary<string, string> { { "Password", "Incorrect password." } } });
            }
            
            var modelErrors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).FirstOrDefault() ?? ""
            );
            return Json(new { success = false, errors = modelErrors });
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser 
                { 
                    UserName = model.Email, 
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "Account created successfully! Please login.", redirectUrl = Url.Action("Login", "Account") });
                }
                
                var error = result.Errors.FirstOrDefault();
                if (error != null)
                {
                    // Map common identity errors to fields
                    if (error.Code.Contains("Email") || error.Code.Contains("UserName"))
                        return Json(new { success = false, errors = new Dictionary<string, string> { { "Email", error.Description } } });
                    if (error.Code.Contains("Password"))
                        return Json(new { success = false, errors = new Dictionary<string, string> { { "Password", error.Description } } });
                    
                    return Json(new { success = false, message = error.Description });
                }
            }
            
            var modelErrors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).FirstOrDefault() ?? ""
            );
            return Json(new { success = false, errors = modelErrors });
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
