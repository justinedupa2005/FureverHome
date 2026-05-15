namespace FureverHome.Models
{
    public class ProfileViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";

        // Ensure this property exists to hold the Image URL
        public string? ProfilePicturePath { get; set; }

        public int ListingsCount { get; set; }
        public int AdoptedCount { get; set; }
    }
}