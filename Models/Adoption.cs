using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FureverHome.Models
{
    public class Adoption
    {
        [Key]
        public int AdoptionID { get; set; }

        [Required]
        public int AdopterID { get; set; }

        [ForeignKey("AdopterID")]
        public virtual Adopter Adopter { get; set; } = null!;

        [Required]
        public int PetID { get; set; }

        [ForeignKey("PetID")]
        public virtual Pet Pet { get; set; } = null!;

        [Required]
        public int StatusID { get; set; }

        [ForeignKey("StatusID")]
        public virtual AdoptionStatus AdoptionStatus { get; set; } = null!;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime AdoptionDate { get; set; } = DateTime.UtcNow;
    }
}
