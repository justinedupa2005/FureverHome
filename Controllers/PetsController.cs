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

        // GET: /Pets/Contact/5
        public IActionResult Contact(int id)
        {
            ViewBag.PetId = id;
            return View();
        }

        // GET: /Pets/Adopt/5
        public IActionResult Adopt(int id)
        {
            ViewBag.PetId = id;
            return View();
        }
    }
}