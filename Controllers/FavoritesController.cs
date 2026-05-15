using FureverHome.Data;
using FureverHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FureverHome.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoritesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Favorites
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");

            var favorites = await _context.Favorites
                .Include(f => f.Pet)
                    .ThenInclude(p => p.Species)
                .Include(f => f.Pet)
                    .ThenInclude(p => p.Breed)
                .Include(f => f.Pet)
                    .ThenInclude(p => p.Gender)
                .Include(f => f.Pet)
                    .ThenInclude(p => p.AdoptionStatus)
                .Include(f => f.Pet)
                    .ThenInclude(p => p.PetOwner)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => f.Pet)
                .ToListAsync();

            ViewData["Title"] = "My Favorite Pets";
            return View(favorites);
        }

        // POST: /Favorites/Toggle
        [HttpPost]
        public async Task<IActionResult> Toggle(int petId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Json(new { success = false, message = "User not logged in" });

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PetID == petId);

            bool isFavorite;
            if (favorite == null)
            {
                _context.Favorites.Add(new Favorite
                {
                    UserId = userId,
                    PetID = petId
                });
                isFavorite = true;
            }
            else
            {
                _context.Favorites.Remove(favorite);
                isFavorite = false;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, isFavorite });
        }
    }
}
