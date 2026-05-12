using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FureverHome.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        // Navigation properties
        public virtual Adopter? AdopterProfile { get; set; }
        public virtual PetOwner? PetOwnerProfile { get; set; }
    }
}
