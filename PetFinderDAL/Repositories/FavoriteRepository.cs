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
   public class FavoriteRepository : IFavoriteRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FavoriteRepository> _logger;

    public FavoriteRepository(AppDbContext context, ILogger<FavoriteRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

        public List<FavoriteList> GetFavoritePets(string userId)
        {
            var listPets = _context.FavoriteList.Where(x => x.ApplicationUser.Id == userId).ToList();

            return listPets;
        }

        public FavoriteList AddFavoritePet(FavoriteList Favorite)
        {
            var newFavoritePet = _context.FavoriteList.Add(Favorite);

            if (newFavoritePet != null && newFavoritePet.State == EntityState.Added)
            {
                var affectedRows = _context.SaveChanges();

                if (affectedRows > 0)
                {
                    return newFavoritePet.Entity;
                }
            }
            return null;

        }

        public FavoriteList RemoveFavoritePet(FavoriteList Favorite)
        {
            var RemoveFavoritePet = _context.FavoriteList.Remove(Favorite);

            if (RemoveFavoritePet != null && RemoveFavoritePet.State == EntityState.Deleted)
            {
                var affectedRows = _context.SaveChanges();

                if (affectedRows > 0)
                {
                    return RemoveFavoritePet.Entity;
                }
            }
            return null;

        }

        public FavoriteList GetFavoritePet(string userId, int petId)
        {
            var favorite = _context.FavoriteList.Where(x => x.ApplicationUser.Id == userId && x.PetId == petId).FirstOrDefault();

            return favorite;
                }
   
    }
}