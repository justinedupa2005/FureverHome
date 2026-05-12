using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FureverHome.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        // GET: /Profile
        public IActionResult Index()
        {
            ViewData["Title"] = "My Profile";
            return View();
        }

        // POST: /Profile/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(string Name, string Email, string Phone, string Address)
        {
            // TODO: Update user profile in database
            return RedirectToAction("Index");
        }
    }
}