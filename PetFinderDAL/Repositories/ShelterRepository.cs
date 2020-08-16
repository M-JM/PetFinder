using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFinderDAL.Context;
using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetFinderDAL.Repositories
{
  public class ShelterRepository : IShelterRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PetRepository> _logger;

        public ShelterRepository(AppDbContext context, ILogger<PetRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public Shelter AddShelter(Shelter shelter)
        {
            var newShelter = _context.Shelters.Add(shelter);

            if (newShelter != null && newShelter.State == EntityState.Added)
            {
                var affectedRows = _context.SaveChanges();

                if (affectedRows > 0)
                {
                    return newShelter.Entity;
                }
            }
            _logger.LogError("There was an error during creation of Shelter");
            return null;

        }

        public Shelter UpdateShelter(Shelter shelter)
        {
            var newShelter = _context.Shelters.Update(shelter);

            if (newShelter != null && newShelter.State == EntityState.Modified)
            {
                var affectedRows = _context.SaveChanges();

                if (affectedRows > 0)
                {
                    return newShelter.Entity;
                }
            }
            _logger.LogError("There was an error during creation of Shelter");
            return null;

        }

        public Shelter GetShelterById(int? id)
        {
            Shelter shelter =_context.Shelters.Find(id);

            return shelter;
        }

    }
}

