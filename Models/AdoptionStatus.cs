using System.ComponentModel.DataAnnotations;

namespace FureverHome.Models
{
    public class AdoptionStatus
    {
        [Key]
        public int StatusID { get; set; }

        [Required]
        [StringLength(50)]
        public string StatusName { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<Adoption> Adoptions { get; set; } = new List<Adoption>();
        public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
    }
}
