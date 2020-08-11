using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetFinderDAL.Models;
using PetFinderDAL.Repositories;
using PetFinder.ViewModels.FavoriteViewModel;
using Microsoft.AspNetCore.Http;

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

    
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult FavoriteList()
        {

            var listofpets = _favoriteRepository.GetFavoritePets(HttpContext.Session.GetString("id"));

            FavoriteListViewModel FavoriteListViewModel = new FavoriteListViewModel()
            {
                FavoriteLists = listofpets
            };

            return View(FavoriteListViewModel);
        }

        public IActionResult AddFavorite(int id)
        {


            Pet pet = _petRepository.GetById(id);
            

            var currentpet = _favoriteRepository.GetFavoritePet(HttpContext.Session.GetString("id"), pet.PetId);
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
            return RedirectToAction("Details","Pet", new { id });
        }


    }
}
