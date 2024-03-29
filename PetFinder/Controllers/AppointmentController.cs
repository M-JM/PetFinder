﻿using System;
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
        // Alternative is creating an Array to map the values from JSON response of method , does not work. -> Mapped event object are not being pushed in Array before the Calender has rendered
        // thus Calendar uses empty Array and return no appointments...

        ///5.Is it ok to instead of Throw to return View Error ? Does it lose important infomation in Dev. env ? (Test this out).
        // the exception is being logged though, so might not be too bad??

        ///6. Make a proper email body -> fix date to only show date and time correctly. -> add shelter info maybe.
        // -> see if there is not an easier way to write the HTML body of the email -> put email sending in seperate service taken body, header, sender as parameters and inject in controller ? 

        ///7. Is there any sense in having more then 3 categories of appointments ?.
        // -> maybe for stat purpose ?

        ///8. Make it that if booking date < DateTime.Now() => automatic assign of category so they do not appear as accepted anymore ?

        ///9.Allow the user to cancel an appointment 
        // The user should be able to cancel an appointment in their list of appointments. The Admin should the be notified that the appointment was cancelled by user (seperate status?)

        ///10. Notification system for users and Admins when new appointments are pending approval , being able to sort by creation date would be good. 

        ///11. Make it possible to have stats for admins ? # of no shows , history of users.
        ///# of favorite pets and how many traffic view ? counter of views as property van pet ? increment telkens de get view word aangeroepen ?

        ///12.(Fixed) Make customer authorization / AuthorizeAttribute.HandleUnauthorizedRequest(AuthorizationContext) Method -> currently when not authorized it redirect to Error controller
        // Which returns the 404 page instead of the not authorized page. In cases where admin of the wrong shelter accesses it is intended that notauthorized be checked against shelterid of the ressource he is trying to view
        

            
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Agenda()
        {
            // this gives the values for statusesdropdown in View 
            // this is required so when the Edit Modal is called 
            // the dropdown is populated with values from DB instead of hardcoding them in a select form-group.
            try
            {
                List<AppointmentStatus> statuses = _appointmentRepository.GetStatus();
                StatusViewModel updateModel = new StatusViewModel(statuses)
                {

                };

                if (User.IsInRole("Admin"))
                {
                    return View(updateModel);
                }
                return View("NotAuthorized");

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"When getting the agenda.");
                throw;
            }
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
                    string emailbodyUser = 
                   "<html><body><p>Dear,</p>" +
                   "Your Booking with " + pet.Name + " has been succesfully received by shelter :  " + pet.Shelter.Name + "."
                    + "<p>Upon confirmation of the shelter , you will receive an email or you can monitor the status in your appointment overview </p>"
                    + "<p>Sincerely,<br>Petfinder Team</br></p>" +
                    "</br><p>this is an automated email , do not reply - for more info contact the shelter at "+ pet.Shelter.Email + "</p>" + " </body> </html>";

                    string emailbodyAdmin =
                "<html><body><p>Dear,</p>" +
                "<p>There is a new booking request that is waiting for approval.</p>"
                + "<p>Sincerely,<br>Petfinder Team</br></p> </body> </html>";
        
                    
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

                    await _emailService.SendAsync(currentuser.UserName, "Appointment - Receipt - PetFinder", emailbodyUser, true);
                    await _emailService.SendAsync(pet.Shelter.Email, "Appointment - Pending Approval - PetFinder", emailbodyAdmin, true);

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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public JsonResult GetAppointments()
        {
            try
            {

                List<Appointment> appointments = _appointmentRepository.GetAppointments(Convert.ToInt32(HttpContext.Session.GetString("shelterid")));
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
                        appointmentstatusId = appointment.AppointmentStatus.AppointmentStatusId,
                        Start = appointment.Date + appointment.StartTime,
                        End = appointment.Date + appointment.EndTime,
                        allDay = false,
                    };
                    Listevents.Add(events);
                };
                                
                return Json(Listevents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"When trying to retrieve appointments.");
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

            try
            {
                Appointment appointment = _appointmentRepository.GetAppointment(Updatedevent.AppointmentId);
                ApplicationUser usertoreceiveEmail = await _userManager.FindByIdAsync(appointment.ApplicationUserId);

                appointment.AppointmentStatusId = Updatedevent.appointmentstatusId;
                Appointment response = _appointmentRepository.UpdateAppointment(appointment);
                Boolean status = false;

                if (response != null) 
                {
                    if(new int[] { 2, 3, }.Contains(response.AppointmentStatusId)) { 
                    string emailbody = "";

                    switch (response.AppointmentStatusId)
                    {
                        case 2:

                            emailbody = "<html><body><p>Dear,</p>" +
                            "<p>Your appointment with " + appointment.Pet.Name + " has been rejected.</p>" +
                            "<p>Please contact " + response.Shelter.Name + " at " + response.Shelter.PhoneNumber + " or email them at " + response.Shelter.Email + " for more information.</p>"
                            + "<p>Sincerely,<br>Petfinder Team</br></p> </body> </html>";

                            break;

                        case 3:

                            emailbody = "<html><body><p>Dear,</p>" +
                            "<p>We hereby confirm your appointment with " + appointment.Pet.Name + ".</p>"
                            + "<p>Please present yourself on " + response.Date.ToShortDateString() + " at " + response.StartTime + " at shelter " + response.Shelter.Name + "</p>"
                            + "<p>Should you want to cancel your appointment please contact " + response.Shelter.Name + " at " + response.Shelter.PhoneNumber + " or email them at " + response.Shelter.Email + "</p>"
                            + "<p>Sincerely,<br>Petfinder Team</br></p> </body> </html>";

                            break;

                        default:

                            break;
                    }

                    await _emailService.SendAsync(usertoreceiveEmail.UserName, "Appointment - PetFinder", emailbody, true);
                    }
                    status = true;
                }

                return Json(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"When trying to save the appointment after choosing status");
                throw;
            }
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Overview()
        {

            try
            {
                List<Appointment> appointments = new List<Appointment>();
           
              appointments = _appointmentRepository.GetAppointments(Convert.ToInt32(HttpContext.Session.GetString("shelterid")));



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

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public IActionResult MyAppointments()
        {

            try
            {
                List<Appointment> appointments = new List<Appointment>();

                if (User.IsInRole("User") && User != null)
                {
                    appointments = _appointmentRepository.GetAppointmentsUsers(HttpContext.Session.GetString("id"));

                }
                else
                {
                    return View("Error");
                }
              

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

        //// Non Functional see-> TODO
        //[HttpGet]
        //[Authorize(Roles = "User")]
        //public JsonResult GetBlockHours(int PetId, DateTime date)
        //{
        //    try
        //    {
        //        List<Appointment> appointments = _appointmentRepository.GetHoursofAppointment(PetId, date);

        //        return Json(appointments);

        //    }
        //    catch (Exception ex)
        //    {

        //        _logger.LogError(ex, $"When trying to retrieve appointments on the given date.");
        //        throw;
        //    }
        //}



    }
}
