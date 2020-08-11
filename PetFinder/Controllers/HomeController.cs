using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetFinder.ViewModels.HomeViewModel;
using PetFinderDAL.Models;
using PetFinderDAL.Repositories;

namespace PetFinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPetRepository _petRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger,
            IPetRepository petRepository,
            IAppointmentRepository appointmentRepository,
            UserManager<ApplicationUser> userManager
            )
        {
            _logger = logger;
            _petRepository = petRepository;
            _appointmentRepository = appointmentRepository;
            _userManager = userManager;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles ="Admin,ShelterUser")]
        public async Task<IActionResult> AdminIndexAsync()
        {
            ApplicationUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            IList<Pet> pets = _petRepository.GetAllPetsFromShelter(user.ShelterId).ToList();
            IList<Appointment> appointments = _appointmentRepository.GetAppointments(user.ShelterId).ToList();
            IList<ApplicationUser> users = _userManager.Users.Where(x => x.ShelterId == user.ShelterId).ToList();

        AdminIndexViewModel viewmodel = new AdminIndexViewModel()
            {
                appointments = appointments,
                Pets = pets,
                Employees = users


            };
            return View(viewmodel);
        }

       
    }
}
