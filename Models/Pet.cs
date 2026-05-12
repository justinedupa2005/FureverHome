using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FureverHome.Models
{
    public class Pet
    {
        [Key]
        public int PetID { get; set; }

        [Required]
        [StringLength(50)]
        public string PetName { get; set; } = string.Empty;

        [Required]
        public int OwnerID { get; set; }

        [ForeignKey("OwnerID")]
        public virtual PetOwner PetOwner { get; set; } = null!;

        [Required]
        public int SpeciesID { get; set; }

        [ForeignKey("SpeciesID")]
        public virtual Species Species { get; set; } = null!;

        [Required]
        public int BreedID { get; set; }

        [ForeignKey("BreedID")]
        public virtual Breed Breed { get; set; } = null!;

        [Required]
        public int GenderID { get; set; }

        [ForeignKey("GenderID")]
        public virtual Gender Gender { get; set; } = null!;

        [Required]
        public int Age { get; set; }

        [Required]
        public bool Vaccinated { get; set; }

        [Required]
        public int StatusID { get; set; }

        [ForeignKey("StatusID")]
        public virtual AdoptionStatus AdoptionStatus { get; set; } = null!;

        // Navigation property
        public virtual ICollection<Adoption> Adoptions { get; set; } = new List<Adoption>();
    }
}
