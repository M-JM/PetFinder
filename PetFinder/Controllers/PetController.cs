﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetFinder.Models;
using PetFinder.ViewModels;
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

        //Asynchronous action methods are useful when an action must perform several independent long running operations. these happen on a seperate thread then the main thread.
        //Making a method asynchronous does not make it execute faster, and that is an important factor to understand and a misconception many people have.


        // TODO 
        // Data validation on models
        // All forms 
        // Check if Int ShelterID given when creating pet is id from shelter
        // implement usernotauthorized

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
        [Authorize(Roles = "Admin,ShelterUser,User")]
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

                //if (!newpets.Any())
                //{
                //    string error = "No pets found";
                //    return Json(error);
                //}

                return Json(newpets);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"When getting the results of the search pets method.");
                throw;
            }         
        }

        [HttpGet]
        [Authorize(Roles = "Admin,ShelterUser")]
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
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ShelterUser")]
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
        [Authorize(Roles = "Admin,ShelterUser")]
        public IActionResult Edit(int id)
        {
            try
            {
                Pet pet = _petRepository.GetById(id);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"When trying to get the edit model for pet.");
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,ShelterUser")]
        public IActionResult Edit(PetEditViewModel editModel)
        {

            try
            {
                Pet pet = _petRepository.GetById(editModel.PetId);
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
                        return RedirectToAction("Details", new { id= pet.PetId});
                    }

                }
                return View(editModel);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"When trying to get the edit model for pet.");
                throw;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,ShelterUser")]
        public IActionResult Delete(int id)
        {
            try
            {
                Pet pet = _petRepository.GetById(id);

                return View(pet);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin,ShelterUser")]
        public IActionResult DeleteSure(int PetId)
        {
            try
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
            catch (Exception)
            {

                throw;
            }

        }

        [HttpGet]
        public IActionResult Details(int id)
        {

            Pet pet = _petRepository.GetById(id);
            Debug.WriteLine(HttpContext.Session.GetString("id"));
            //var currentuser = await _userManager.GetUserAsync(HttpContext.User);
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

        //Move the CalculateAge Method in Viewmodel ? 
        // This should be doable ... seperate concern (MVC pattern) -> View data representation goes in viewmodel not controller...


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
