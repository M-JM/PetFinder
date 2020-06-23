using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFinderDAL.Context;
using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetFinderDAL.Repositories
{
   public class PetRepository : IPetRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PetRepository> _logger;

        public PetRepository(AppDbContext context, ILogger<PetRepository> logger)
        {
            _context = context;
            _logger = logger;
        }


        // Create - Pet

        public Pet AddPet (Pet pet)
        {
            var newPet = _context.Pets.Add(pet);

            if (newPet != null && newPet.State == EntityState.Added)
            {
                var affectedRows = _context.SaveChanges();

                if (affectedRows > 0)
                {
                    return newPet.Entity;
                }
            }

            return null;

        }

        // Create - PetPictures

        public PetPicture AddPetPicture(PetPicture petPicture)
        {
            var newPetPicture = _context.PetPictures.Add(petPicture);

            if(newPetPicture != null && newPetPicture.State == EntityState.Added)
            {
                var affectedRows = _context.SaveChanges();
                if (affectedRows > 0)
                {
                    return newPetPicture.Entity;
                }
            }
            return null;
        }


        //Read

        // Get - PetDetails

            public Pet GetById(int id) { 

                var pet = _context.Pets
                .Include(p => p.PetColor)
                .Include(p => p.PetRace)
                .Include(p => p.PetKind)
                .Include(p => p.PetPictures)
                .Include(p => p.Shelter).FirstOrDefault(m => m.PetId == id);

            return pet;
        }

        // Get - All Pets

        public IEnumerable<Pet> GetAllPets()
        {
            var listPets = _context.Pets.ToList();

            return listPets;
        }

        // Get - PetColors

        public IEnumerable<PetColor> GetPetColors()
        {
            var PetcolorList = _context.PetColors.ToList();

            return PetcolorList;

        }

        // Get - PetRaces

        public IEnumerable<PetRace> GetPetRaces()
        {
            var PetRaceList = _context.PetRaces.ToList();
        
            return PetRaceList;

        }

        // Get - PetKinds
        public IEnumerable<PetKind> GetPetKinds()
        {
            var PetKindList = _context.PetKind.ToList();

            return PetKindList;

        }


        //Update

        //Delete

        public Pet RemovePet(Pet pet)
        {
            var removePet = _context.Pets.Remove(pet);

            if (removePet != null && removePet.State == EntityState.Deleted)
            {
                var affectedRows = _context.SaveChanges();

                if (affectedRows > 0)
                {
                    _logger.LogInformation($"The {pet.Name} was deleted.");
                    return removePet.Entity;
                }
            }

            return null;

        }

        //public PetPicture RemovePetPicture(PetPicture petPicture)
        //{
        //    var deletePetPicture = _context.PetPictures.Remove(petPicture);

        //    if ( deletePetPicture.State == EntityState.Deleted)
        //    {
        //       _context.SaveChanges();
               
        //    }
        //    return null;
        //}

    }
}
