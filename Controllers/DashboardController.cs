using FureverHome.Data;
using FureverHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FureverHome.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public DashboardController(ApplicationDbContext context,
                                   UserManager<ApplicationUser> userManager,
                                   IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        // GET: /Dashboard
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var owner = await _context.PetOwners
                .Include(po => po.Pets)
                    .ThenInclude(p => p.Species)
                .Include(po => po.Pets)
                    .ThenInclude(p => p.Breed)
                .Include(po => po.Pets)
                    .ThenInclude(p => p.Gender)
                .Include(po => po.Pets)
                    .ThenInclude(p => p.AdoptionStatus)
                .FirstOrDefaultAsync(po => po.UserId == user.Id);

            ViewData["Title"] = "My Dashboard";
            return View(owner?.Pets ?? new List<Pet>());
        }

        // GET: /Dashboard/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.Users
                .Include(u => u.AdopterProfile)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null) return RedirectToAction("Login", "Account");

            // Pass profile info to the view for pre-filling
            ViewBag.PhoneNumber = user.AdopterProfile?.PhoneNumber ?? user.PhoneNumber;
            ViewBag.Address = user.AdopterProfile?.Address;

            ViewData["Title"] = "List a Pet";
            return View();
        }

        // POST: /Dashboard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Name, string Type, string Breed, string Age,
                                    string Gender, string ContactNumber, string Address,
                                    string Vaccinated, string Description, IFormFile? Photo)
        {
            var user = await _userManager.Users
                .Include(u => u.AdopterProfile)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null) return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(Breed) || string.IsNullOrEmpty(Address))
            {
                ViewBag.PhoneNumber = user.AdopterProfile?.PhoneNumber ?? user.PhoneNumber;
                ViewBag.Address = user.AdopterProfile?.Address;
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return View();
            }

            try
            {
                var owner = await _context.PetOwners.FirstOrDefaultAsync(po => po.UserId == user.Id);
                if (owner == null)
                {
                    owner = new PetOwner
                    {
                        UserId = user.Id,
                        FirstName = user.FirstName ?? user.UserName?.Split('@')[0] ?? "User",
                        LastName = user.LastName ?? "Owner",
                        PhoneNumber = ContactNumber ?? user.PhoneNumber ?? ""
                    };
                    _context.PetOwners.Add(owner);
                    await _context.SaveChangesAsync();
                }

                var speciesName = char.ToUpper(Type[0]) + Type.Substring(1).ToLower();
                var species = await _context.Species.FirstOrDefaultAsync(s => s.SpeciesName == speciesName);
                if (species == null)
                {
                    species = new Species { SpeciesName = speciesName };
                    _context.Species.Add(species);
                    await _context.SaveChangesAsync();
                }

                var breedObj = await _context.Breeds.FirstOrDefaultAsync(b => b.BreedName.ToLower() == Breed.ToLower() && b.SpeciesID == species.SpeciesID);
                if (breedObj == null)
                {
                    breedObj = new Breed { BreedName = Breed, SpeciesID = species.SpeciesID };
                    _context.Breeds.Add(breedObj);
                    await _context.SaveChangesAsync();
                }

                var genderObj = await _context.Genders.FirstOrDefaultAsync(g => g.GenderName.ToLower() == Gender.ToLower());
                if (genderObj == null)
                {
                    genderObj = await _context.Genders.FirstOrDefaultAsync() ?? new Gender { GenderName = "Unknown" };
                    if (genderObj.GenderID == 0)
                    {
                        _context.Genders.Add(genderObj);
                        await _context.SaveChangesAsync();
                    }
                }

                var status = await _context.AdoptionStatuses.FirstOrDefaultAsync(s => s.StatusName == "Available")
                             ?? await _context.AdoptionStatuses.FirstOrDefaultAsync()
                             ?? new AdoptionStatus { StatusName = "Available" };

                if (status.StatusID == 0)
                {
                    _context.AdoptionStatuses.Add(status);
                    await _context.SaveChangesAsync();
                }

                string? imagePath = null;
                if (Photo != null && Photo.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "pets");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Photo.CopyToAsync(fileStream);
                    }
                    imagePath = "/images/pets/" + uniqueFileName;
                }

                var pet = new Pet
                {
                    PetName = Name,
                    OwnerID = owner.OwnerID,
                    SpeciesID = species.SpeciesID,
                    BreedID = breedObj.BreedID,
                    GenderID = genderObj.GenderID,
                    Age = Age,
                    Address = Address,
                    Vaccinated = (Vaccinated?.ToLower() == "yes"),
                    Description = Description,
                    ImagePath = imagePath,
                    StatusID = status.StatusID
                };

                _context.Pets.Add(pet);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Pet listed successfully! 🐾";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while saving the pet: " + ex.Message;
                return View();
            }
        }

        // GET: /Dashboard/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var pet = await _context.Pets
                .Include(p => p.Species)
                .Include(p => p.Breed)
                .Include(p => p.Gender)
                .Include(p => p.AdoptionStatus)
                .Include(p => p.PetOwner)
                .FirstOrDefaultAsync(p => p.PetID == id);

            if (pet == null) return NotFound();

            var owner = await _context.PetOwners.FirstOrDefaultAsync(po => po.UserId == user.Id);
            if (owner == null || pet.OwnerID != owner.OwnerID) return Forbid();

            ViewData["Title"] = "Edit Listing";
            return View(pet);
        }

        // POST: /Dashboard/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Name, string Type, string Breed, string Age,
                                  string Gender, string ContactNumber, string Address, string Status,
                                  string Vaccinated, string Description, IFormFile? Photo)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(Breed) || string.IsNullOrEmpty(Address) || string.IsNullOrEmpty(Status))
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToAction("Edit", new { id });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            try
            {
                var pet = await _context.Pets.Include(p => p.PetOwner).FirstOrDefaultAsync(p => p.PetID == id);
                if (pet == null) return NotFound();

                var owner = await _context.PetOwners.FirstOrDefaultAsync(po => po.UserId == user.Id);
                if (owner == null || pet.OwnerID != owner.OwnerID) return Forbid();

                if (!string.IsNullOrEmpty(ContactNumber))
                {
                    owner.PhoneNumber = ContactNumber;
                    _context.PetOwners.Update(owner);
                }

                // Update Status Mapping
                var statusObj = await _context.AdoptionStatuses.FirstOrDefaultAsync(s => s.StatusName == Status);
                if (statusObj != null)
                {
                    pet.StatusID = statusObj.StatusID;
                }

                var speciesName = char.ToUpper(Type[0]) + Type.Substring(1).ToLower();
                var species = await _context.Species.FirstOrDefaultAsync(s => s.SpeciesName == speciesName);
                if (species == null)
                {
                    species = new Species { SpeciesName = speciesName };
                    _context.Species.Add(species);
                    await _context.SaveChangesAsync();
                }

                var breedObj = await _context.Breeds.FirstOrDefaultAsync(b => b.BreedName.ToLower() == Breed.ToLower() && b.SpeciesID == species.SpeciesID);
                if (breedObj == null)
                {
                    breedObj = new Breed { BreedName = Breed, SpeciesID = species.SpeciesID };
                    _context.Breeds.Add(breedObj);
                    await _context.SaveChangesAsync();
                }

                var genderObj = await _context.Genders.FirstOrDefaultAsync(g => g.GenderName.ToLower() == Gender.ToLower());
                if (genderObj != null)
                {
                    pet.GenderID = genderObj.GenderID;
                }

                if (Photo != null && Photo.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "pets");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Photo.CopyToAsync(fileStream);
                    }
                    pet.ImagePath = "/images/pets/" + uniqueFileName;
                }

                pet.PetName = Name;
                pet.SpeciesID = species.SpeciesID;
                pet.BreedID = breedObj.BreedID;
                pet.Age = Age;
                pet.Address = Address;
                pet.Vaccinated = Vaccinated.ToLower() == "yes";
                pet.Description = Description;

                _context.Pets.Update(pet);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Pet listing updated successfully! 🐾";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the pet: " + ex.Message;
                return RedirectToAction("Edit", new { id });
            }
        }

        // GET: /Dashboard/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var pet = await _context.Pets.FindAsync(id);
            if (pet == null) return NotFound();

            var owner = await _context.PetOwners.FirstOrDefaultAsync(po => po.UserId == user.Id);
            if (owner == null || pet.OwnerID != owner.OwnerID) return Forbid();

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}