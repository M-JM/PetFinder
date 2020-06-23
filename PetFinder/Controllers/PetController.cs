using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PetFinder.Models;
using PetFinder.ViewModels;
using PetFinderDAL.Models;
using PetFinderDAL.Repositories;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetFinder.Controllers
{
    public class PetController : Controller
    {
        private readonly IPetRepository _petRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<PetController> _logger;

        public PetController(IPetRepository petRepository,
            IWebHostEnvironment WebHostEnvironment,
             ILogger<PetController> logger)
        {
            _petRepository = petRepository;
            _webHostEnvironment = WebHostEnvironment;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var petList = _petRepository.GetAllPets();
                return View(petList);
            }
            
             catch (Exception ex)
            {
                _logger.LogError(ex, $"When retrieving Company List.");
                throw;

            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            var PetColorList = _petRepository.GetPetColors();
            var PetRaceList = _petRepository.GetPetRaces();
            var PetKindList = _petRepository.GetPetKinds();

            PetCreateViewModel CreateModel = new PetCreateViewModel()
            {
                PetColorList = PetColorList.Select(r =>
                new SelectListItem()
                {
                     Value = r.PetColorId.ToString(),
                     Text = r.Color
                }),

                PetRaceList = PetRaceList.Select(r =>
                new SelectListItem()
                {
                    Value = r.PetRaceId.ToString(),
                    Text = r.RaceName
                }),

                PetKindList = PetKindList.Select(r =>
                new SelectListItem()
                {
                    Value = r.PetKindId.ToString(),
                    Text = r.AnimalType
                })
            };
            return View(CreateModel);
        }
        [HttpPost]
        public IActionResult Create(PetCreateViewModel createmodel)
        {
            if(ModelState.IsValid)
            {
                List<string> uniqueFilenames = ProcessUploadFile(createmodel);

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

                foreach (var picture in uniqueFilenames)
                {
                    PetPicture newPicture = new PetPicture
                    {
                        Pet = newPet,
                        PhotoPath = picture
                    };
                    _petRepository.AddPetPicture(newPicture);
                }
                return RedirectToAction("Details", new { id = newPet.PetId });
            }
            return View(createmodel);

        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var pet = _petRepository.GetById(id);

            PetEditViewModel editModel = new PetEditViewModel()
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
            var pet = _petRepository.GetById(id);

            return View(pet);
        }
        [HttpPost]
        public IActionResult DeleteSure(int PetId)
        {
            var pet = _petRepository.GetById(PetId);
            var response = _petRepository.RemovePet(pet);

            if (response != null && response.PetId != 0)
            {
                //foreach( var picture in pet.PetPictures)
                //{
                //    _petRepository.RemovePetPicture(picture);
                //}
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var pet = _petRepository.GetById(id);

            PetDetailViewModel detailViewModel = new PetDetailViewModel()
            {
                Pet = pet
            };

            return View(detailViewModel);
        }

        private List<string> ProcessUploadFile(PetCreateViewModel createmodel)
        {
            List<string> uniqueFilenames = new List<string>();
            foreach (var Photo in createmodel.PetPictures)
            {
                if (createmodel.PetPictures != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

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

    }
}
