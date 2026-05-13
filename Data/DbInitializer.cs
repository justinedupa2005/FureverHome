using FureverHome.Models;
using Microsoft.EntityFrameworkCore;

namespace FureverHome.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any species.
            if (context.Species.Any())
            {
                return;   // DB has been seeded
            }

            var species = new Species[]
            {
                new Species { SpeciesName = "Dog" },
                new Species { SpeciesName = "Cat" },
                new Species { SpeciesName = "Bird" },
                new Species { SpeciesName = "Rabbit" },
                new Species { SpeciesName = "Other" }
            };
            context.Species.AddRange(species);
            context.SaveChanges();

            var genders = new Gender[]
            {
                new Gender { GenderName = "Male" },
                new Gender { GenderName = "Female" }
            };
            context.Genders.AddRange(genders);
            context.SaveChanges();

            var statuses = new AdoptionStatus[]
            {
                new AdoptionStatus { StatusName = "Available" },
                new AdoptionStatus { StatusName = "Adopted" },
                new AdoptionStatus { StatusName = "Pending" }
            };
            context.AdoptionStatuses.AddRange(statuses);
            context.SaveChanges();

            // Seed some initial breeds for the species
            var dogSpecies = context.Species.First(s => s.SpeciesName == "Dog");
            var catSpecies = context.Species.First(s => s.SpeciesName == "Cat");

            var breeds = new Breed[]
            {
                new Breed { SpeciesID = dogSpecies.SpeciesID, BreedName = "Labrador" },
                new Breed { SpeciesID = dogSpecies.SpeciesID, BreedName = "Golden Retriever" },
                new Breed { SpeciesID = dogSpecies.SpeciesID, BreedName = "Bulldog" },
                new Breed { SpeciesID = catSpecies.SpeciesID, BreedName = "Persian" },
                new Breed { SpeciesID = catSpecies.SpeciesID, BreedName = "Siamese" },
                new Breed { SpeciesID = catSpecies.SpeciesID, BreedName = "Maine Coon" }
            };
            context.Breeds.AddRange(breeds);
            context.SaveChanges();
        }
    }
}
