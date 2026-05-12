using System.ComponentModel.DataAnnotations;

namespace FureverHome.Models
{
    public class Gender
    {
        [Key]
        public int GenderID { get; set; }

        [Required]
        [StringLength(20)]
        public string GenderName { get; set; } = string.Empty;

        // Navigation property
        public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
    }
}
