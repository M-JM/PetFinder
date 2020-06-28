using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFinderDAL.Context;
using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetFinderDAL.Repositories
{
   public class LocationRepository : ILocationRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PetRepository> _logger;

        public LocationRepository(AppDbContext context, ILogger<PetRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Location Addlocation(Location location)
        {
            var newLocation = _context.Locations.Add(location);

            if (newLocation != null && newLocation.State == EntityState.Added)
            {
                var affectedRows = _context.SaveChanges();

                if (affectedRows > 0)
                {
                    return newLocation.Entity;
                }
            }
            _logger.LogError("There was an error during creation of Location");
            return null;

        }
    }
}
