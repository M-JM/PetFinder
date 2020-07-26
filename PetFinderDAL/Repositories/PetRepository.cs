using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NinjaNye.SearchExtensions;
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

            //method GetbyId /datatype Class Pet - take petId as parameter (given with the controller when calling method)
            // var pet = open DB pets( include table , color , race, kind , pictures, shelter ( all of these tables have relation with Pet)
           
               Pet pet = _context.Pets
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

        public List<PetColor> GetPetColors()
        {
            var PetcolorList = _context.PetColors.ToList();

            return PetcolorList;

        }

        // Get - PetRaces

        public List<PetRace> GetPetRaces()
        {
            var PetRaceList = _context.PetRaces.ToList();
        
            return PetRaceList;

        }

        // Get - PetKinds
        public List<PetKind> GetPetKinds()
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

        public IEnumerable<Pet> GetSearchedPets(SearchModel searchmodel)
        {
            //IEnumerable<Pet> pets = _context.Pets
            //     .Include(p => p.PetColor)
            //     .Include(p => p.PetRace)
            //     .Include(p => p.PetKind)
            //     .Include(p => p.PetPictures)
            //     .Include(p => p.Shelter).Where(x => searchmodel.Size.Contains(x.Size)).ToList();


            //return pets;
            //https://weblogs.asp.net/zeeshanhirani/using-asqueryable-with-linq-to-objects-and-linq-to-sql
            //https://stackoverflow.com/questions/33153932/filter-search-using-multiple-fields-asp-net-mvc


            var result = _context.Pets.Include(p => p.PetColor)
                 .Include(p => p.PetRace)
                 .Include(p => p.PetKind)
                 .Include(p => p.PetPictures)
                 .Include(p => p.Shelter).AsQueryable();

            if (searchmodel != null)
            {
                if(searchmodel.Appartmentfit != null)
                    result = result.Where(x => x.Appartmentfit == searchmodel.Appartmentfit);
                if (searchmodel.KidsFriendly != null)
                    result = result.Where(x => x.KidsFriendly == searchmodel.KidsFriendly);
                if (searchmodel.SocialWithCats != null)
                    result = result.Where(x => x.SocialWithCats == searchmodel.SocialWithCats);
                if (searchmodel.SocialWithDogs != null)
                    result = result.Where(x => x.SocialWithDogs == searchmodel.SocialWithDogs);
                if (searchmodel.Gender.Count != 0)
                    result = result.Where(x => searchmodel.Gender.Contains(x.Gender));
                if (searchmodel.PetColorId.Count != 0)
                    result = result.Where(x => searchmodel.PetColorId.Contains(x.PetColorId));
                if (searchmodel.PetKindId.Count != 0)
                    result = result.Where(x => searchmodel.PetKindId.Contains(x.PetKindId));
                if (searchmodel.PetRaceId.Count != 0)
                    result = result.Where(x => searchmodel.PetRaceId.Contains(x.PetRaceId));
                
            }
            return result.ToList();

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
