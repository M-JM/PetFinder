using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetFinderDAL.Repositories
{
    public interface IFavoriteRepository
    {
        List<FavoriteList> GetFavoritePets(string userId);
        FavoriteList GetFavoritePet(string userId, int petId);
        FavoriteList AddFavoritePet(FavoriteList Favorite);
        FavoriteList RemoveFavoritePet(FavoriteList Favorite);
        public bool FavoriteExists(string userId, int petId);
    }
}
