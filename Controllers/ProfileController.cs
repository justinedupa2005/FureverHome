using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Required for .Include()
using FureverHome.Models;
using FureverHome.Data; // Replace with your actual namespace for ApplicationDbContext

namespace FureverHome.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            // 1. Get the ID of the currently logged-in user
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Fetch user data including the Adopter profile for the Address
            var user = await _context.Users
                .Include(u => u.AdopterProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            // 3. Map the database data to the ViewModel
            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                // Accessing Address from the Adopter navigation property
                Address = user.AdopterProfile?.Address ?? "No address on file"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProfileViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _context.Users
                .Include(u => u.AdopterProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            // --- EMAIL UPDATE LOGIC ---
            if (user.Email != model.Email)
            {
                // Check if the new email is already taken by someone else
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("Email", "This email is already in use.");
                    return View("Index", model);
                }

                user.Email = model.Email;
                user.UserName = model.Email; // Keep Username and Email in sync
                user.NormalizedEmail = model.Email.ToUpper();
                user.NormalizedUserName = model.Email.ToUpper();
            }

            // --- OTHER FIELDS ---
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            if (user.AdopterProfile == null)
            {
                user.AdopterProfile = new Adopter
                {
                    UserId = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber
                };
                _context.Adopters.Add(user.AdopterProfile);
            }
            else
            {
                user.AdopterProfile.FirstName = model.FirstName;
                user.AdopterProfile.LastName = model.LastName;
                user.AdopterProfile.Address = model.Address;
                user.AdopterProfile.PhoneNumber = model.PhoneNumber;
            }

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes.");
                return View("Index", model);
            }
        }
    }
}