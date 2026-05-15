using FureverHome.Data;
using FureverHome.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FureverHome.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var petCount = await _context.Pets.CountAsync();
            ViewBag.PetCount = petCount;

            var featuredPets = await _context.Pets
                .Include(p => p.Species)
                .Include(p => p.Breed)
                .Include(p => p.Gender)
                .Include(p => p.AdoptionStatus)
                .OrderByDescending(p => p.PetID)
                .Take(3)
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
            ViewBag.FavoritePetIds = favoritePetIds;

            return View(featuredPets);
        }

        public IActionResult About()
        {
            ViewData["Title"] = "About Us";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Title"] = "Contact Us";
            return View();
        }

        public IActionResult AdoptionProcess()
        {
            ViewData["Title"] = "Adoption Process";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}