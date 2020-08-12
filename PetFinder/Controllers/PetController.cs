using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetFinder.ViewModels.PetViewModel;
using PetFinderDAL.Models;
using PetFinderDAL.Repositories;

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

        //INFO

        /// Async methods 
        //Asynchronous action methods are useful when an action must perform several independent long running operations. these happen on a seperate thread then the main thread.
        //Making a method asynchronous does not make it execute faster, and that is an important factor to understand and a misconception many people have.

        /// ANTIFORGERY -> Prevent Cross site request.
        //MVC's anti-forgery support writes a unique value to an HTTP-only cookie and then the same value is written to the form.
        //When the page is submitted, an error is raised if the cookie value doesn't match the form value.

        //TODO 

        //From high priority to low 

        ///1.Edit model must contain a method to delete existing photos from DB & WWWroot.
        //   in html implement Ajax call to delete methode and call it on span/link item on each existing photo
        //   the Method will take the photopath as parameter and retrieve the selected photo and delete it from DB and WWWROOT.

        ///2. Move the CalculateAge Method in Viewmodel ? 
        // This should be doable ... seperate concern (MVC pattern) -> View data representation goes in viewmodel not controller...

        ///3. Log warning when triggering the notauthorized View call to log who tried to access the ressources ??

        ///4. Check to encrypt and decrypt parameters being passed. 
        // there are no known sensitive data being passed as parameters , but it might be good pratice to do this anyways.


        ///5. Refactor the Search method. using Business layer (DTO) to map values from Viewmodel passed then as parameter to SearchModel and pass it to DAL
        //   Search for a cleaner solution to pass values , ugly to loop over every property of viewmodel to map them searchmodel.
        //   Can this be done in JQ ? and pass then through Ajax the data object to Method ??

        ///6. Move Uploadfunctionality to a Service with an Iservice to inject it in controller where i would use this method ??
       
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
        [Authorize(Roles = "Admin, User")]
        public IActionResult Search()
        {
            
            try
            {
                IEnumerable<Pet> petList = _petRepository.GetAllPets();
                List<PetColor> PetColorList = _petRepository.GetPetColors();
                List<PetRace> PetRaceList = _petRepository.GetPetRaces();
                List<PetKind> PetKindList = _petRepository.GetPetKinds();

                SearchViewModel CreateModel = new SearchViewModel(PetColorList, PetKindList, PetRaceList)
                {
                    ListOfPets = petList,
                };

                return View(CreateModel);
            }

            catch (Exception ex)

            {
                _logger.LogError(ex, $"When getting the create pet form.");
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetSearchedPets(SearchViewModel model)
        {
            try
            {
                List<string> SizelistSearch = new List<string>();
                List<int?> SearchPetKindId = new List<int?>();
                List<int> SearchPetColorId = new List<int>();
                List<int> SearchPetRaceId = new List<int>();
                List<string> SearchGender = new List<string>();
                List<string> SearchAge = new List<string>();


                IEnumerable<Pet> petList = _petRepository.GetAllPets();

                foreach (var item in model.SizeList)
                {
                    if (item.Selected)
                    {
                        SizelistSearch.Add(item.Value);
                    }
                };
                if (model.PetKindIdfromForm != null) { 
                    foreach (var item in model.PetKindIdfromForm)                   
                    {
                        SearchPetKindId.Add(item);
                    }
                };

                foreach (var item in model.PetColorList)
                {
                    if (item.Selected)
                    {
                        SearchPetColorId.Add(Int32.Parse(item.Value));
                    }
                };
                foreach (var item in model.PetRaceList)
                {
                    if (item.Selected)
                    {
                        SearchPetRaceId.Add(Int32.Parse(item.Value));
                    }
                };
                foreach (var item in model.Genderlist)
                {
                    if (item.Selected)
                    {
                        SearchGender.Add(item.Value);
                    }
                };

                SearchModel searchModel = new SearchModel
                {
                    Gender = SearchGender,
                    Size = SizelistSearch,
                    PetColorId = SearchPetColorId,
                    PetKindId = SearchPetKindId,
                    PetRaceId = SearchPetRaceId,
                    Appartmentfit = model.Appartmentfit,
                    KidsFriendly = model.KidsFriendly,
                    SocialWithCats = model.SocialWithCats,
                    SocialWithDogs = model.SocialWithDogs,
                };
                    IEnumerable<Pet> newpets = _petRepository.GetSearchedPets(searchModel);

                return Json(newpets);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"When getting the results of the search pets method.");
                throw;
            }         
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
          try
            {

                List<PetColor> PetColorList = _petRepository.GetPetColors();
                List<PetRace> PetRaceList = _petRepository.GetPetRaces();
                List<PetKind> PetKindList = _petRepository.GetPetKinds();

                PetCreateViewModel CreateModel = new PetCreateViewModel(PetColorList, PetKindList, PetRaceList)
                {
                    DOB = DateTime.Now,          
                };
                return View(CreateModel);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"When getting the create pet form.");

                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(PetCreateViewModel createmodel)
        {
            try

            {
                if (ModelState.IsValid)
                {
                    ApplicationUser user = await _userManager.FindByEmailAsync(User.Identity.Name);
                    int? shelterid = user.ShelterId;

                    Pet newPet = new Pet
                    {
                        Name = createmodel.Name,
                        Description = createmodel.Description,
                        DOB = createmodel.DOB,
                        Gender = createmodel.Gender,
                        PetColorId = createmodel.PetColorId,
                        PetKindId = createmodel.PetKindId,
                        ShelterId = shelterid,
                        Size = createmodel.Size,
                        PetRaceId = createmodel.PetRaceId,
                        SocialWithCats = createmodel.SocialWithCats,
                        SocialWithDogs = createmodel.SocialWithDogs,
                        Appartmentfit = createmodel.Appartmentfit,
                        KidsFriendly = createmodel.KidsFriendly,                                   
                    };
                    _petRepository.AddPet(newPet);

                    if (createmodel.PetPictures != null)
                    {
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
                    else
                    {
                        return RedirectToAction("Details", new { id = newPet.PetId });
                    }
                    return RedirectToAction("Details", new { id = newPet.PetId });
                }
                return View(createmodel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"When trying to create pet.");
                throw;
            }


        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            try
            {
                Pet pet = _petRepository.GetById(id);
                
                if (pet.ShelterId == Convert.ToInt32(HttpContext.Session.GetString("shelterid")))
                {
                    List<PetColor> PetColorList = _petRepository.GetPetColors();
                    List<PetRace> PetRaceList = _petRepository.GetPetRaces();
                    List<PetKind> PetKindList = _petRepository.GetPetKinds();


                    PetEditViewModel editModel = new PetEditViewModel(PetColorList, PetKindList, PetRaceList)
                    {

                        PetId = pet.PetId,
                        Name = pet.Name,
                        Description = pet.Description,
                        DOB = pet.DOB,
                        Gender = pet.Gender,
                        PetColorId = pet.PetColorId,
                        PetKindId = pet.PetKindId,
                        ShelterId = pet.ShelterId,
                        Size = pet.Size,
                        PetRaceId = pet.PetRaceId,
                        SocialWithCats = pet.SocialWithCats,
                        SocialWithDogs = pet.SocialWithDogs,
                        KidsFriendly = pet.KidsFriendly,
                        Appartmentfit = pet.Appartmentfit,

                    };
                    return View(editModel);
                }
                // implement logging warning when triggering "Not auhtorized" together with Id of user ??
                                
                return View("NotAuthorized");
               

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"When trying to get the edit model for pet.");
                return View("error");
            }
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(PetEditViewModel editModel)
        {

            try
            {
                Pet pet = _petRepository.GetById(editModel.PetId);
                if (pet.ShelterId == Convert.ToInt32(HttpContext.Session.GetString("shelterid")))
                {
                    if (ModelState.IsValid)
                    {
                        pet.Name = editModel.Name;
                        pet.Description = editModel.Description;
                        pet.DOB = editModel.DOB;
                        pet.Gender = editModel.Gender;
                        pet.PetColorId = editModel.PetColorId;
                        pet.PetKindId = editModel.PetKindId;
                        pet.Size = editModel.Size;
                        pet.PetRaceId = editModel.PetRaceId;
                        pet.SocialWithCats = editModel.SocialWithCats;
                        pet.SocialWithDogs = editModel.SocialWithDogs;
                        pet.KidsFriendly = editModel.KidsFriendly;
                        pet.Appartmentfit = editModel.Appartmentfit;

                        var response = _petRepository.EditPet(pet);

                        if (response != null & response.PetId != 0)
                        {
                            return RedirectToAction("Details", new { id = pet.PetId });
                        }
                    }
                    return View(editModel);
                }
                return View("NotAuthorized");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"When trying to get the edit model for pet.");
                return View("error");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                Pet pet = _petRepository.GetById(id);

                return View(pet);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"When trying to get the delete page for pet.");
                return View("error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteSure(int PetId)
        {
            try
            {
                Pet pet = _petRepository.GetById(PetId);
                Pet response = _petRepository.RemovePet(pet);

                if (response != null && response.PetId != 0)
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"When trying to delete a pet.");
                return View("error");
            }

        }

        [HttpGet]
        public IActionResult Details(int id)
        {

            try
            {
                Pet pet = _petRepository.GetById(id);
                if (pet == null)
                {
                    return View("Error");
                }
                bool isFavorite = _favoriteRepository.FavoriteExists(HttpContext.Session.GetString("id"), pet.PetId);
                string age = CalculateAge(pet.DOB);

                PetDetailViewModel detailViewModel = new PetDetailViewModel()
                {
                    Pet = pet,
                    Isfavorite = isFavorite,
                    Age = age,
                };

                return View(detailViewModel);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"When trying to get the Detail for pet.");
                return View("error");
            }
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

        private static string CalculateAge(DateTime dateOfBirth)
        {
            string Age ;

            string years = (DateTime.Now.Year - dateOfBirth.Year).ToString();
            string months = (DateTime.Now.Month - dateOfBirth.Month).ToString();

            if(years == "0")
            {
                Age = months + " months";
            }
            else if(years =="1")
            {
                Age = years + " year " + months + " months";
            }
            else
            {
                Age = years + " years " + months + " months";
            }
                      
            return Age;

            //Method to get Age as a string to pass to detail of pet viewmodel. takes DOB as parameter and perform simple substraction of Years and months based on date of today
            // If else statement could be replace by switch/cases 
      
            // Private populate list method. ( parameter)

            // TODO 
            // Move getuser in seperate method to call only when required.
            // Use session cookies as alternative to retrieve current user data. 
            // Expiration date - key/value principles to add data

        }
    }
}
