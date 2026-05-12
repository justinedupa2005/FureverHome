using Microsoft.AspNetCore.Mvc;

namespace FureverHome.Controllers
{
    public class DashboardController : Controller
    {
        // GET: /Dashboard
        public IActionResult Index()
        {
            ViewData["Title"] = "My Dashboard";
            return View();
        }

        // GET: /Dashboard/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "List a Pet";
            return View();
        }

        // POST: /Dashboard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string Name, string Type, string Breed, string Age,
                                    string Gender, string ContactNumber, string Vaccinated,
                                    string Description, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                // TODO: Save pet listing to database
                // TODO: Handle photo upload to wwwroot/images/pets/
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: /Dashboard/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewData["Title"] = "Edit Listing";
            ViewBag.PetId = id;
            return View();
        }

        // POST: /Dashboard/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string Name, string Type, string Breed, string Age,
                                  string Gender, string ContactNumber, string Vaccinated,
                                  string Description, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                // TODO: Update pet listing in database
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: /Dashboard/Delete/5
        public IActionResult Delete(int id)
        {
            // TODO: Delete pet listing from database
            return RedirectToAction("Index");
        }
    }
}