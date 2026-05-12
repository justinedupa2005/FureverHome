using System.ComponentModel.DataAnnotations;

namespace FureverHome.Models
{
    public class Species
    {
        [Key]
        public int SpeciesID { get; set; }

        [Required]
        [StringLength(50)]
        public string SpeciesName { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<Breed> Breeds { get; set; } = new List<Breed>();
        public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
    }
}
