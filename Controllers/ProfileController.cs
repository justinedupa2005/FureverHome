using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FureverHome.Models;
using FureverHome.Data;

namespace FureverHome.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _context.Users
                .Include(u => u.AdopterProfile)
                .Include(u => u.PetOwnerProfile)
                    .ThenInclude(po => po != null ? po.Pets : null)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.AdopterProfile?.Address ?? "No address on file",
                // MAP THE PATH TO THE VIEWMODEL
                ProfilePicturePath = user.AdopterProfile?.ProfilePicturePath,
                ListingsCount = user.PetOwnerProfile?.Pets.Count ?? 0
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
                .Include(u => u.PetOwnerProfile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            // Email Sync
            if (user.Email != model.Email)
            {
                user.Email = model.Email;
                user.UserName = model.Email;
                user.NormalizedEmail = model.Email.ToUpper();
                user.NormalizedUserName = model.Email.ToUpper();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            // Sync Adopter Profile
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

            // Sync Pet Owner Profile
            if (user.PetOwnerProfile != null)
            {
                user.PetOwnerProfile.FirstName = model.FirstName;
                user.PetOwnerProfile.LastName = model.LastName;
                user.PetOwnerProfile.PhoneNumber = model.PhoneNumber;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePhoto(IFormFile ProfilePicture)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");

            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                // 1. Ensure folder exists
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "profiles");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                // 2. Save file
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ProfilePicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePicture.CopyToAsync(fileStream);
                }

                // 3. Database Update
                var user = await _context.Users
                    .Include(u => u.AdopterProfile)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user != null)
                {
                    if (user.AdopterProfile == null)
                    {
                        // Create profile if it doesn't exist
                        user.AdopterProfile = new Adopter
                        {
                            UserId = user.Id,
                            FirstName = user.FirstName ?? "User",
                            LastName = user.LastName ?? "Name",
                            PhoneNumber = user.PhoneNumber ?? "000",
                            Address = "Default Address",
                            ProfilePicturePath = "/images/profiles/" + uniqueFileName
                        };
                        _context.Adopters.Add(user.AdopterProfile);
                    }
                    else
                    {
                        // Update existing profile
                        user.AdopterProfile.ProfilePicturePath = "/images/profiles/" + uniqueFileName;
                        _context.Adopters.Update(user.AdopterProfile);
                    }

                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}