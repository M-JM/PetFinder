using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PetFinder.Models;
using PetFinder.ViewModels;
using PetFinder.ViewModels.FavoriteViewModel;
using PetFinderDAL.Models;
using PetFinderDAL.Repositories;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetFinder.Controllers
{
    public class PetController : Controller
    {
        private readonly IPetRepository _petRepository;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<PetController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public PetController(IPetRepository petRepository,
            IFavoriteRepository favoriteRepository,
            IWebHostEnvironment WebHostEnvironment,
             ILogger<PetController> logger,
              UserManager<ApplicationUser> userManager)
        {
            _petRepository = petRepository;
            _favoriteRepository = favoriteRepository;
            _webHostEnvironment = WebHostEnvironment;
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            try
            {
                IEnumerable<Pet> petList = _petRepository.GetAllPets();
                return View(petList);
            }
            
             catch (Exception ex)
            {
                _logger.LogError(ex, $"When retrieving Pets List.");
                throw;

            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            List<PetColor> PetColorList = _petRepository.GetPetColors();
            List<PetRace> PetRaceList = _petRepository.GetPetRaces();
            List<PetKind> PetKindList = _petRepository.GetPetKinds();
           
            PetCreateViewModel CreateModel = new PetCreateViewModel(PetColorList, PetKindList, PetRaceList)
            {

              
            };
            return View(CreateModel);
        }
        [HttpPost]
        public IActionResult Create( PetCreateViewModel createmodel)
        {
            if(ModelState.IsValid)
            {
               
                Pet newPet = new Pet
                {
                    Name = createmodel.Name,
                    Description = createmodel.Description,
                    DOB = createmodel.DOB,
                    Gender = createmodel.Gender,
                    PetColorId = createmodel.PetColorId,
                    PetKindId = createmodel.PetKindId,
                    ShelterId = createmodel.ShelterId,
                    Size = createmodel.Size,
                    PetRaceId = createmodel.PetRaceId,
                    Social = createmodel.Social
                };
                _petRepository.AddPet(newPet);

                if (createmodel.PetPictures != null ) {
                    List<string> uniqueFilenames = ProcessUploadFile(createmodel);

                    foreach (var picture in uniqueFilenames)
                    {
                        PetPicture newPicture = new PetPicture
                        {
                            Pet = newPet,
                            PhotoPath = picture
                        };
                        _petRepository.AddPetPicture(newPicture);
                    }
                }
                else { 
                return RedirectToAction("Details", new { id = newPet.PetId });
                }
            }
            return View(createmodel);

        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Pet pet = _petRepository.GetById(id);
            List<PetColor> PetColorList = _petRepository.GetPetColors();
            List<PetRace> PetRaceList = _petRepository.GetPetRaces();
            List<PetKind> PetKindList = _petRepository.GetPetKinds();


            PetEditViewModel editModel = new PetEditViewModel(PetColorList, PetKindList, PetRaceList)
            {

                Name = pet.Name,
                Description = pet.Description,
                DOB = pet.DOB,
                Gender = pet.Gender,
                PetColorId = pet.PetColorId,
                PetKindId = pet.PetKindId,
                ShelterId = pet.ShelterId,
                Size = pet.Size,
                PetRaceId = pet.PetRaceId,
                Social = pet.Social
            };
            return View(editModel);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Pet pet = _petRepository.GetById(id);

            return View(pet);
        }
        [HttpPost]
        public IActionResult DeleteSure(int PetId)
        {
            Pet pet = _petRepository.GetById(PetId);
            Pet response = _petRepository.RemovePet(pet);

            if (response != null && response.PetId != 0)
            {
                // the pictures are automatically deleted due to cascade in DB
                //foreach( var picture in pet.PetPictures)
                //{
                //    _petRepository.RemovePetPicture(picture);
                //}
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            Pet pet = _petRepository.GetById(id);
            var currentuser = await _userManager.GetUserAsync(HttpContext.User);
            bool isFavorite = _favoriteRepository.FavoriteExists(currentuser.Id, pet.PetId);

            PetDetailViewModel detailViewModel = new PetDetailViewModel()
            {
                Pet = pet,
                Isfavorite = isFavorite


            };

            return View(detailViewModel);
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
            else { 

            NewFavoriteViewModel model = new NewFavoriteViewModel()
            {
                ApplicationUser = currentuser,
                Pet = pet,
              
            };

            _favoriteRepository.AddFavoritePet(model);
            }
            return RedirectToAction("Details", new { id });
        }




        private List<string> ProcessUploadFile(PetCreateViewModel createmodel)
        {
            List<string> uniqueFilenames = new List<string>();
            foreach (IFormFile Photo in createmodel.PetPictures)
            {
                if (createmodel.PetPictures != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                    Photo.CopyTo(fileStream);
                    uniqueFilenames.Add(uniqueFileName);
                }
            }
            return uniqueFilenames;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Private populate list method. ( parameter)

    }
}
