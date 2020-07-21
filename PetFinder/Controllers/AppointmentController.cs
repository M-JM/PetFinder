using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PetFinder.ViewModels.AppointmentViewModel;
using PetFinderDAL.Models;
using PetFinderDAL.Repositories;

namespace PetFinder.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IPetRepository _petRepository;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<PetController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentController(IPetRepository petRepository,
            IFavoriteRepository favoriteRepository,
            IWebHostEnvironment WebHostEnvironment,
             ILogger<PetController> logger,
              UserManager<ApplicationUser> userManager,
              IAppointmentRepository appointmentRepository)
        {
            _petRepository = petRepository;
            _favoriteRepository = favoriteRepository;
            _webHostEnvironment = WebHostEnvironment;
            _logger = logger;
            _userManager = userManager;
            _appointmentRepository = appointmentRepository;
        }

        public IActionResult List()
        {
            List<AppointmentStatus> statuses =_appointmentRepository.GetStatus();

            StatusViewModel updateModel = new StatusViewModel(statuses)
            {
            };

            return View(updateModel);

        }
        [HttpGet]
        public async Task<IActionResult> CreateAsync(int petid)
        {
            Pet pet = _petRepository.GetById(petid);
            var currentuser = await _userManager.GetUserAsync(HttpContext.User);

            AppointmentCreateViewModel model = new AppointmentCreateViewModel()
            {
                PetId = pet.PetId,
                AppointmentStatusId = 1,
                ApplicationUserId = currentuser.Id,
                ShelterId = pet.Shelter.ShelterId,   
            };


            return View(model);
        }

        [HttpPost]
        public IActionResult Create(AppointmentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                TimeSpan endTime = model.StartTime.Add(new TimeSpan(1, 0, 0));

                Appointment appointment= new Appointment
                {
                   StartTime = model.StartTime,
                   EndTime = endTime,
                   Date = model.Date,
                   ApplicationUserId = model.ApplicationUserId,
                   ShelterId = model.ShelterId,
                   PetId =model.PetId,
                   AppointmentStatusId = model.AppointmentStatusId
                               
                   

                };
                _appointmentRepository.AddAppointment(appointment);

             
            }
            return View(model);
        }

        public JsonResult GetAppointments()
        {
            List<Appointment> appointments = _appointmentRepository.GetAppointments();
            List<CalendarEvents> Listevents = new List<CalendarEvents>();


            foreach (var appointment in appointments)
            {
                CalendarEvents events = new CalendarEvents
                {
                    AppointmentId = appointment.AppointmentId,
                    Title = appointment.Pet.Name,
                    User = appointment.ApplicationUser.Email,
                    BackgroundColor = appointment.AppointmentStatus.Color,
                    Status = appointment.AppointmentStatus.StatusName,
                    Start = appointment.Date + appointment.StartTime,
                    End = appointment.Date + appointment.EndTime,
                    allDay = false,
                };
                Listevents.Add(events);
            };
            
          //  var json = JsonConvert.SerializeObject(appointments);
          //  return new JsonResult(json);

            return Json(Listevents);
            }

        [HttpPost]
        public JsonResult SaveEvent(CalendarEvents Updatedevent)
        {

            Appointment appointment = _appointmentRepository.GetAppointment(Updatedevent.AppointmentId);

            appointment.AppointmentStatusId = Updatedevent.appointmentstatusId;

       
            _appointmentRepository.UpdateAppointment(appointment);
            var status = true;

            return Json(status);
        }
    }
}
     