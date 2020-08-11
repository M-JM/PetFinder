using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Core;
using PetFinder.ViewModels.AppointmentViewModel;
using PetFinderDAL.Models;
using PetFinderDAL.Repositories;

namespace PetFinder.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IPetRepository _petRepository;
     
        private readonly ILogger<PetController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IEmailService _emailService;

        public AppointmentController(
            IPetRepository petRepository,
            ILogger<PetController> logger,
            UserManager<ApplicationUser> userManager,
            IAppointmentRepository appointmentRepository,
            IEmailService emailService)
        {
            _petRepository = petRepository;
            _logger = logger;
            _userManager = userManager;
            _appointmentRepository = appointmentRepository;
            _emailService = emailService;
        }

        public IActionResult List()
        {
            List<AppointmentStatus> statuses = _appointmentRepository.GetStatus();
            // this gives the values for statusesdropdown in View 
            // this is required so when the Edit Modal is called , the dropdown is populated with values from DB instead of hardcoding them in a select form-group.
            StatusViewModel updateModel = new StatusViewModel(statuses)
            {
            };

            return View(updateModel);

        }
        [HttpGet]
        public IActionResult Create(int petid)
        {
            Pet pet = _petRepository.GetById(petid);
            AppointmentCreateViewModel model = new AppointmentCreateViewModel()
            {
                PetId = pet.PetId,
                AppointmentStatusId = 1,
                // Any appointment created has the automatic status of Pending
                ApplicationUserId = HttpContext.Session.GetString("id"),
                ShelterId = pet.Shelter.ShelterId,
                Date = DateTime.Now,
                StartTime = new TimeSpan(8, 00, 00),

                // Date and Startime are set in the createviewModel to have initial values in inputfields
                // Since both values are non-nullable C# automatically assign them a default value ( default(DateTime) & default(StartTime))
                // Making them nullable will allow to put placeholders like DD-MM-YY & HH-MM in the inputfields to have be able to perform validation on model
                // Currently if the user saves accidently the form without selecting their own values , it will pass validation of the POST method.
                //THIS IS NOT ACCEPTABLE -> changing the Appointment model in DAL to nullable requires changes across all methods involved.
                // by making it required i would still make sure no nulls value are saved in DB ( bad practice ?)
                // maybe make a nullable in Viewmodel with other name and not direct property inherited from appointment would work.( bad pratice ??)
            };


            return PartialView("Create",model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(AppointmentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pet = _petRepository.GetById(model.PetId);
                var currentuser =  await _userManager.FindByIdAsync(HttpContext.Session.GetString("id"));
                string emailbody = "Your Booking with " + pet.Name + " has been succesfully received";

                // instead of adding 1 hour through built in method -> change into adding one hour to end date in form before post.

                TimeSpan endTime = model.StartTime.Add(new TimeSpan(1, 0, 0));

                Appointment appointment = new Appointment
                {
                    StartTime = model.StartTime,
                    EndTime = endTime,
                    Date = model.Date,
                    ApplicationUserId = model.ApplicationUserId,
                    ShelterId = model.ShelterId,
                    PetId = model.PetId,
                    AppointmentStatusId = model.AppointmentStatusId



                };
                
               _appointmentRepository.AddAppointment(appointment);

                await _emailService.SendAsync(currentuser.UserName, "Appointment - PetFinder", emailbody, true);


                return RedirectToAction("Details","Pet", new { id = appointment.PetId });
            
            }
            return View(model);
        }

        public async Task<JsonResult> GetAppointmentsAsync()
        {
            ApplicationUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            List<Appointment> appointments = _appointmentRepository.GetAppointments(user.ShelterId);
            List<CalendarEvents> Listevents = new List<CalendarEvents>();


            foreach (var appointment in appointments)
            {
                // Calendar lib takes a well defined class to show the events.
                // Appointment class could be modified so the properties have the correct name so Calendar gets JSON feed with proper property name
                // This would avoid to have to make new CalendarEvents with repetition of Data that is at 90% already available through class Appointment...

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

            // Avoid doing the above  and passing the json in the JSON method return, as JSON() already serialize the object given as parameter.
            // having two serialization causes the JSON structure to be different Array of Array with Index and value(array of objects )thus requires unnecessary long pathing to get to values of properties 

            return Json(Listevents);
        }

        [HttpPost]
        public async Task<JsonResult> SaveEventAsync(CalendarEvents Updatedevent)
        {
            // the object given as parameter with SaveEvent comes from the AJAX call (post) with the data from the EditModal
            // change the status 
            // the return of this method is a JSON containing the value of the boleaan in order to check the succes function in the script of the view.

            Appointment appointment = _appointmentRepository.GetAppointment(Updatedevent.AppointmentId);
            ApplicationUser usertoreceiveEmail = await _userManager.FindByIdAsync(appointment.ApplicationUserId);

                appointment.AppointmentStatusId = Updatedevent.appointmentstatusId;
            string emailbody ="";


            var response = _appointmentRepository.UpdateAppointment(appointment);
            
            if( response != null)
            {
                switch (response.AppointmentStatusId) { 

                    case 3:

                        emailbody = "Your Appointment with " + appointment.Pet.Name + " at " + appointment.Shelter.Name + " on " + response.Date + " has been confirmed";

                    break;

                    case 2:

                        emailbody = "Your Appointment with " + appointment.Pet.Name + " at " + appointment.Shelter.Name + " on " + response.Date + " has been Rejected";

                        break;

                }

            }

            await _emailService.SendAsync(usertoreceiveEmail.UserName, "Appointment - PetFinder", emailbody, true);

            Boolean status = true;

            return Json(status);
        }

        [HttpGet]

        public JsonResult GetBlockHours(int PetId, DateTime date)
        {
           var appointments =  _appointmentRepository.GetHoursofAppointment(PetId, date);

            return Json(appointments);

        }

    }
}
