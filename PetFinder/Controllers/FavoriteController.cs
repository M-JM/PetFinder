using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetFinderDAL.Models;
using PetFinderDAL.Repositories;
using PetFinder.ViewModels.FavoriteViewModel;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetFinder.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IPetRepository _petRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FavoriteController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoriteController(IFavoriteRepository favoriteRepository,
            IPetRepository petRepository,
            IWebHostEnvironment WebHostEnvironment,
             ILogger<FavoriteController> logger,
              UserManager<ApplicationUser> userManager)
        {
            _favoriteRepository = favoriteRepository;
            _petRepository = petRepository;
            _webHostEnvironment = WebHostEnvironment;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> FavoriteList()
        {

            var currentuser = await _userManager.GetUserAsync(HttpContext.User);
            var listofpets = _favoriteRepository.GetFavoritePets(currentuser.Id);

            FavoriteListViewModel FavoriteListViewModel = new FavoriteListViewModel()
            {
                FavoriteLists = listofpets
            };

            return View(FavoriteListViewModel);
        }

        public async Task<IActionResult> AddFavoriteAsync(int id)
        {
            Pet pet = _petRepository.GetById(id);
            var currentuser = await _userManager.GetUserAsync(HttpContext.User);
            var currentpet = _favoriteRepository.GetFavoritePet(currentuser.Id, pet.PetId);
            if (currentpet != null)
            {
                _favoriteRepository.RemoveFavoritePet(currentpet);
            }
            else
            {

                NewFavoriteViewModel model = new NewFavoriteViewModel()
                {
                    ApplicationUser = currentuser,
                    Pet = pet,

                };

                _favoriteRepository.AddFavoritePet(model);
            }
            return RedirectToAction("Details","Pet", new { id });
        }


    }
}
