using FureverHome.Data;
using FureverHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FureverHome.Controllers
{
    [Authorize]
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Pets
        public async Task<IActionResult> Index(string? filter)
        {
            var pets = await _context.Pets
                .Include(p => p.Species)
                .Include(p => p.Breed)
                .Include(p => p.Gender)
                .Include(p => p.AdoptionStatus)
                .Include(p => p.PetOwner)
                    .ThenInclude(po => po.User)
                .ToListAsync();

            ViewData["Title"] = "Pets Looking for a Home";
            ViewBag.Filter = filter;
            return View(pets);
        }

        // GET: /Pets/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var pet = await _context.Pets
                .Include(p => p.Species)
                .Include(p => p.Breed)
                .Include(p => p.Gender)
                .Include(p => p.AdoptionStatus)
                .Include(p => p.PetOwner)
                    .ThenInclude(po => po.User)
                .FirstOrDefaultAsync(p => p.PetID == id);

            if (pet == null)
            {
                return NotFound();
            }

            ViewData["Title"] = pet.PetName + " - Details";
            return View(pet);
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