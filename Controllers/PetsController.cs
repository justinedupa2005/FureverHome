using Microsoft.AspNetCore.Mvc;

namespace FureverHome.Controllers
{
    public class PetsController : Controller
    {
        // GET: /Pets
        public IActionResult Index(string? filter)
        {
            ViewData["Title"] = "Pets Looking for a Home";
            ViewBag.Filter = filter;
            return View();
        }

        // GET: /Pets/Details/5
        public IActionResult Details(int id)
        {
            ViewData["Title"] = "Pet Details";
            ViewBag.PetId = id;
            return View();
        }

        // Instead of looking for a missing view,
        // just send the user to the Details page
        public IActionResult Contact(int id)
        {
            return RedirectToAction("Details", new { id });
        }

        // Same here — Details page already has the adopt button
        public IActionResult Adopt(int id)
        {
            return RedirectToAction("Details", new { id });
        }
    }
}