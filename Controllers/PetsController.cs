using FureverHome.Data;
using FureverHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FureverHome.Controllers
{
    [Authorize]
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PetsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

            var userId = _userManager.GetUserId(User);
            var favoritePetIds = new List<int>();
            if (userId != null)
            {
                favoritePetIds = await _context.Favorites
                    .Where(f => f.UserId == userId)
                    .Select(f => f.PetID)
                    .ToListAsync();
            }

            ViewData["Title"] = "Pets Looking for a Home";
            ViewBag.Filter = filter;
            ViewBag.FavoritePetIds = favoritePetIds;
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
                        .ThenInclude(u => u.AdopterProfile)
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