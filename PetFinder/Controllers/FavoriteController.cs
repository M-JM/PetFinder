﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetFinderDAL.Models;
using PetFinderDAL.Repositories;
using PetFinder.ViewModels.FavoriteViewModel;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace PetFinder.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IPetRepository _petRepository;
        private readonly ILogger<FavoriteController> _logger;
       
        public FavoriteController
        (

            IFavoriteRepository favoriteRepository,
            IPetRepository petRepository,
            ILogger<FavoriteController> logger
        )
        {
            _favoriteRepository = favoriteRepository;
            _petRepository = petRepository;
            _logger = logger;
        }

        //TODO 

        //From high priority to low 

        /// 1. fix the layout of the favorite button.
        // the user should be able to favorite pets from the search function and not have to specifically go to the detail of pet page.
        // make it a heart icon ( empty/fill) and use AJAX to call method so no refresh happens on the page. 

        ///2. Implement sort of donation system ? (Check out stripe - sort of free payment gateway).
    

        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult FavoriteList()
        {
            try
            {
                List<FavoriteList> listofpets = _favoriteRepository.GetFavoritePets(HttpContext.Session.GetString("id"));

                FavoriteListViewModel FavoriteListViewModel = new FavoriteListViewModel()
                {
                    FavoriteLists = listofpets
                };

                return View(FavoriteListViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"When trying to retrieve Favorite list.");
                return View("error");
            }
        }

        [Authorize(Roles = "User")]
        public IActionResult AddFavorite(int id)
        {
            try
            {
                Pet pet = _petRepository.GetById(id);

                FavoriteList currentpet = _favoriteRepository.GetFavoritePet(HttpContext.Session.GetString("id"), pet.PetId);
                if (currentpet != null)
                {
                    _favoriteRepository.RemoveFavoritePet(currentpet);
                }
                else
                {
                    NewFavoriteViewModel model = new NewFavoriteViewModel()
                    {
                        ApplicationUserId = HttpContext.Session.GetString("id"),
                        PetId = pet.PetId,
                    };
                    _favoriteRepository.AddFavoritePet(model);
                }
                return RedirectToAction("Details", "Pet", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"When trying to add to favoritelist.");

                return View("error");
            }
        }
    }
}
