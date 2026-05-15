using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FureverHome.Models
{
    public class Favorite
    {
        [Key]
        public int FavoriteID { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [Required]
        public int PetID { get; set; }

        [ForeignKey("PetID")]
        public virtual Pet Pet { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
