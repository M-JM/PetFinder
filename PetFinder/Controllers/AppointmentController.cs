using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        //INFO

        //https://stackify.com/csharp-exception-handling-best-practices/
        // NullExceptionhandling is caught by the try catch. Brief look into info tells that try catch are expensive ressource wise (The entire stack has to be unwound).
        // The info Null check vs. Try catch is pretty dated >5y, more recent info could not directly be found
        // Topic to further investigate and understand what the best pratice is for NullException handling
        // -> intresting is also "error monitoring service" i.e a tool (check out) 

        //Explicit and Implicit Typing

        // There are situation where implicit Type is useful -> anonymous types and LINQ -> when type of variable is not known or does not exist (investigate further)
        // Else always use Explicit type of variable -> better readability and maintainability.

        //TODO 

        //From high priority to low 

        /// 1. Get the hours to block selectlistitems of TimePicker.
        // Find out why Datepicker does not fire function to pass Date to GetHours Method ???
        // The rest of method is straight foward -> get date from Datepicker , send it via AJAX post to method gethours.
        // Retrieve list of hours provided by method gethours , push in Array in JS and set this Array to the property blocked hours of Timepicker.

        /// 2. Fix placeholders for date/time in create booking
        // Date and Startime are set in the createviewModel to have initial values in inputfields
        // Since both values are non-nullable C# automatically assign them a default value ( default(DateTime) & default(StartTime))
        // Making them nullable will allow to put placeholders like DD-MM-YY & HH-MM in the inputfields to have be able to perform validation on model
        // Currently if the user saves accidently the form without selecting their own values , it will pass validation of the POST method.
        //-> changing the Appointment model in DAL to nullable requires changes across all methods involved.
        // by making it required i would still make sure no nulls value are saved in DB ( bad practice ?)
        // maybe make a nullable in Viewmodel with other name and not direct property inherited from appointment would work.( bad pratice ??)

        ///3. Fix Enddate on Create Post method
        // instead of adding 1 hour through built in method -> change into adding one hour to end date in form before post.

        ///4. Refactor GetEvents for Calendar
        // Calendar lib takes a well defined class to show the events.
        // Appointment class could be modified so the properties have the correct name so Calendar gets JSON feed with proper property name
        // This would avoid to have to make new CalendarEvents with repetition of Data that is at 90% already available through class Appointment...
        // Alternative is creating an Array to map the values from response of method , does not work. -> Mapped event object are not being pushed in Array before the Calender has rendered
        // thus Calendar uses empty Array and return no appointments...

        ///5.Is it ok to instead of Throw to return View Error ? Does it lose important infomation in Dev. env ? (Test this out).
        // the exception is being logged though, so might not be to bad??

        ///6. Make a proper email body -> fix date to only show date and time correctly. -> add shelter info maybe.
       
        ///7. Is there any sense in having more then 3 categories of appointments ?.
        // -> maybe for stat purpose ?

        ///8. Make it that if booking date < DateTime.Now() => automatic assign of category so they do not appear as accepted anymore ?
        
        public IActionResult List()
        {
            // this gives the values for statusesdropdown in View 
            // this is required so when the Edit Modal is called 
            // the dropdown is populated with values from DB instead of hardcoding them in a select form-group.
            List<AppointmentStatus> statuses = _appointmentRepository.GetStatus();
            StatusViewModel updateModel = new StatusViewModel(statuses)
            {
            
            };

            return View(updateModel);
        }
        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult Create(int petid)
        {
            try
            {
                Pet pet = _petRepository.GetById(petid);

                AppointmentCreateViewModel model = new AppointmentCreateViewModel()
                {
                    // Any appointment created has the automatic status of Pending
                    PetId = pet.PetId,
                    AppointmentStatusId = 1,
                    ApplicationUserId = HttpContext.Session.GetString("id"),
                    ShelterId = pet.Shelter.ShelterId,
                    Date = DateTime.Now,
                    StartTime = new TimeSpan(8, 00, 00),
                };
                return PartialView("Create", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"When trying to create an appointment.");
                return View("error");
             
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateAsync(AppointmentCreateViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Pet pet = _petRepository.GetById(model.PetId);
                    ApplicationUser currentuser = await _userManager.FindByIdAsync(HttpContext.Session.GetString("id"));
                    string emailbody = "Your Booking with " + pet.Name + " has been succesfully received";

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

                    return RedirectToAction("Details", "Pet", new { id = appointment.PetId });

                }
                return View(model);
            }
            catch (Exception ex )
            {
                _logger.LogError(ex, $"When trying to create an appointment / post method.");
                return View("error");
            }
        }
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> GetAppointmentsAsync()
        {
            try
            {
                ApplicationUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                List<Appointment> appointments = _appointmentRepository.GetAppointments(user.ShelterId);
                List<CalendarEvents> Listevents = new List<CalendarEvents>();


                foreach (Appointment appointment in appointments)
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
                // Avoid doing the above  and passing the json in the JSON method return, as JSON() already serialize the object given as parameter.
                // having two serialization causes the JSON structure to be different Array of Array with Index and value(array of objects )thus requires unnecessary long pathing to get to values of properties 

                return Json(Listevents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"When trying to retrieve appointments / post method.");
                throw;
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<JsonResult> SaveEventAsync(CalendarEvents Updatedevent)
        {
            // the object given as parameter with SaveEvent comes from the AJAX call (post) with the data from the EditModal
            // change the status 
            // the return of this method is a JSON containing the value of the boleaan in order to check the succes function in the script of the view.

            Appointment appointment = _appointmentRepository.GetAppointment(Updatedevent.AppointmentId);
            ApplicationUser usertoreceiveEmail = await _userManager.FindByIdAsync(appointment.ApplicationUserId);

            appointment.AppointmentStatusId = Updatedevent.appointmentstatusId;
            Appointment response = _appointmentRepository.UpdateAppointment(appointment);
            Boolean status = false;

            if (response != null && new int[] { 2, 3, }.Contains(response.AppointmentStatusId))
            {
                string emailbody ="";

                switch (response.AppointmentStatusId)
                {

                    case 3:

                      emailbody ="<html><body><p>Dear,</p>" +
                      "<p>We hereby confirm your appointment with " + appointment.Pet.Name + ".</p>"
                      +"<p>Please present yourself on " + response.Date.ToShortDateString() + " at " + response.StartTime + "</p>"
                     +"<p>Sincerely,<br>Petfinder Team</br></p> </body> </html>";

                        //"Your Appointment with " + appointment.Pet.Name + " at " + appointment.Shelter.Name + " on " + response.Date + " has been confirmed";

                        break;

                    case 2:

                        emailbody = "Your Appointment with " + appointment.Pet.Name + " at " + appointment.Shelter.Name + " on " + response.Date + " has been Rejected";
                        
                        break;
             
                }

                await _emailService.SendAsync(usertoreceiveEmail.UserName, "Appointment - PetFinder", emailbody, true);

            }

            if (response != null)
            {
                status = true;
            }

           

            return Json(status);
        }


        public IActionResult Overview()
        {

            try
            {
                List<Appointment> appointments = _appointmentRepository.GetAppointments(Convert.ToInt32(HttpContext.Session.GetString("shelterid")));

                AppointmentListViewModel model = new AppointmentListViewModel()
                {
                    Appointments = appointments,
                };


                return View(model);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"When trying to retrieve appointments.");
                throw;
            }
        }


        // Non Functional see-> TODO
        [HttpGet]
        public JsonResult GetBlockHours(int PetId, DateTime date)
        {
            List<Appointment> appointments = _appointmentRepository.GetHoursofAppointment(PetId, date);

            return Json(appointments);

        }



    }
}
