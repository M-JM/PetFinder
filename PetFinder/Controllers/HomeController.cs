using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        public HomeController(
            ILogger<HomeController> logger,
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

        //TODO 

        //From high priority to low 

        ///1.Pass Startweek and Endweek as parameters to Appointment repo instead of linq where statement on getallappointments.
     

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles ="Admin,ShelterUser")]
        public  IActionResult AdminIndex()
        {
            try
            {
                
                int shelterid = Convert.ToInt32(HttpContext.Session.GetString("shelterid"));
                DateTime startOfWeek = DateTime.Today;
                int delta = DayOfWeek.Monday - startOfWeek.DayOfWeek;
                startOfWeek = startOfWeek.AddDays(delta);
                DateTime endOfWeek = startOfWeek.AddDays(7);


                IList <Pet> pets = _petRepository.GetAllPetsFromShelter(shelterid).ToList();
                IList<Appointment> appointments = _appointmentRepository.GetAppointments(shelterid).Where(x => x.Date >= startOfWeek && x.Date < endOfWeek).ToList();
                IList<ApplicationUser> users = _userManager.Users.Where(x => x.ShelterId == shelterid).ToList();

                AdminIndexViewModel viewmodel = new AdminIndexViewModel()
                {
                    appointments = appointments,
                    Pets = pets,
                    Employees = users


                };
                return View(viewmodel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"When trying to get AdminIndex.");
                return View("error");
            }
        }

       
    }
}
