using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FureverHome.Models
{
    public class Breed
    {
        [Key]
        public int BreedID { get; set; }

        [Required]
        public int SpeciesID { get; set; }

        [ForeignKey("SpeciesID")]
        public virtual Species Species { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string BreedName { get; set; } = string.Empty;

        // Navigation property
        public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
    }
}
