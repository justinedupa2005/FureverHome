using FureverHome.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FureverHome.Data
{
    // Inheriting from IdentityUserContext provides necessary internal mappings for SignInManager
    // while still avoiding Roles, Claims, and Tokens tables in the physical database
    public class ApplicationDbContext : IdentityUserContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Adopter> Adopters { get; set; }
        public DbSet<AdoptionStatus> AdoptionStatuses { get; set; }
        public DbSet<Adoption> Adoptions { get; set; }
        public DbSet<PetOwner> PetOwners { get; set; }
        public DbSet<Species> Species { get; set; }
        public DbSet<Breed> Breeds { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Favorite -> ApplicationUser (One-to-Many)
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Favorite -> Pet (One-to-Many)
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Pet)
                .WithMany()
                .HasForeignKey(f => f.PetID)
                .OnDelete(DeleteBehavior.Restrict);

            // To avoid the "mapped then ignored" warning, we DON'T use Ignore<T>.
            // IdentityUserContext already handles the exclusion of Roles.
            // Claims, Logins, and Tokens remain in the model (required by SignInManager)
            // but we can choose not to create tables for them if we really wanted to, 
            // however, SignInManager needs them to exist in the metadata.

            // ApplicationUser -> Adopter (One-to-One)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.AdopterProfile)
                .WithOne(a => a.User)
                .HasForeignKey<Adopter>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ApplicationUser -> PetOwner (One-to-One)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.PetOwnerProfile)
                .WithOne(po => po.User)
                .HasForeignKey<PetOwner>(po => po.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Adopter -> Adoption (One-to-Many)
            modelBuilder.Entity<Adoption>()
                .HasOne(a => a.Adopter)
                .WithMany(ad => ad.Adoptions)
                .HasForeignKey(a => a.AdopterID)
                .OnDelete(DeleteBehavior.Cascade);

            // Pet -> Adoption (One-to-Many)
            modelBuilder.Entity<Adoption>()
                .HasOne(a => a.Pet)
                .WithMany(p => p.Adoptions)
                .HasForeignKey(a => a.PetID)
                .OnDelete(DeleteBehavior.Restrict);

            // AdoptionStatus -> Adoption (One-to-Many)
            modelBuilder.Entity<Adoption>()
                .HasOne(a => a.AdoptionStatus)
                .WithMany(s => s.Adoptions)
                .HasForeignKey(a => a.StatusID)
                .OnDelete(DeleteBehavior.Restrict);

            // PetOwner -> Pet (One-to-Many)
            modelBuilder.Entity<Pet>()
                .HasOne(p => p.PetOwner)
                .WithMany(po => po.Pets)
                .HasForeignKey(p => p.OwnerID)
                .OnDelete(DeleteBehavior.Cascade);

            // Species -> Pet (One-to-Many)
            modelBuilder.Entity<Pet>()
                .HasOne(p => p.Species)
                .WithMany(s => s.Pets)
                .HasForeignKey(p => p.SpeciesID)
                .OnDelete(DeleteBehavior.Restrict);

            // Breed -> Pet (One-to-Many)
            modelBuilder.Entity<Pet>()
                .HasOne(p => p.Breed)
                .WithMany(b => b.Pets)
                .HasForeignKey(p => p.BreedID)
                .OnDelete(DeleteBehavior.Restrict);

            // Gender -> Pet (One-to-Many)
            modelBuilder.Entity<Pet>()
                .HasOne(p => p.Gender)
                .WithMany(g => g.Pets)
                .HasForeignKey(p => p.GenderID)
                .OnDelete(DeleteBehavior.Restrict);

            // AdoptionStatus -> Pet (One-to-Many)
            modelBuilder.Entity<Pet>()
                .HasOne(p => p.AdoptionStatus)
                .WithMany(s => s.Pets)
                .HasForeignKey(p => p.StatusID)
                .OnDelete(DeleteBehavior.Restrict);

            // Species -> Breed (One-to-Many)
            modelBuilder.Entity<Breed>()
                .HasOne(b => b.Species)
                .WithMany(s => s.Breeds)
                .HasForeignKey(b => b.SpeciesID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
