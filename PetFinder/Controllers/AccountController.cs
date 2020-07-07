using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PetFinder.ViewModels.AccountViewModel;
using PetFinderDAL.Models;
using PetFinderDAL.Repositories;

namespace PetFinder.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ILocationRepository _locationRepository;
        private readonly IShelterRepository _shelterRepository;
        private const string _baseUrl = "https://maps.googleapis.com/maps/api/geocode/json?address=a&key=";

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            ILocationRepository locationRepository,
            IShelterRepository shelterRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _locationRepository = locationRepository;
            _shelterRepository = shelterRepository;
        }

        private async Task<string> GetGeoAsync()
        {
            using HttpClient client = new HttpClient();
            HttpResponseMessage responseBody = await client.GetAsync(_baseUrl);
            var reply = await responseBody.Content.ReadAsStringAsync();
            return reply;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (signInResult.Succeeded)
                {
                    if (string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    var currentuser = await _userManager.FindByEmailAsync(model.Email);

                    //if (User.IsInRole("Admin"))

                    if (await _userManager.IsInRoleAsync(currentuser, "Admin"))
                    {
                        return RedirectToAction("AdminIndex", "Home");
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            UserRegisterViewModel Registermodel = new UserRegisterViewModel
            {
    
            };
            
            return View(Registermodel);
        }
        [HttpGet]
        public IActionResult ShelterRegister()
        {
            ShelterRegisterViewModel Registermodel = new ShelterRegisterViewModel
            {

            };

            return View(Registermodel);
        }
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(UserRegisterViewModel Registermodel)
        {
         
            using HttpClient client = new HttpClient();

            //WORKING CODE
            //var testingasstring= await client.GetAsync(_baseUrl);
            //var result = testingasstring.Content.ReadAsStringAsync().Result;
            //GoogleApi.Rootobject google = JsonConvert.DeserializeObject<GoogleApi.Rootobject>(result);
            //var testing = google.results.FirstOrDefault();
            // ENDWORKING CODE

            //var testingasJson = await client.GetFromJsonAsync<GoogleApi>(_baseUrl);
            //var test = await testingasstring.Content.ReadAsStringAsync();

            // Entire API call to google needs to be rewritten into a service with method accepting Concatnated Address parameter
            // In case the address return no object -> use City ZIP CODE to get at least approx. geocoding
            // Update the Location class with ZIP CODE property
            // Temporarly removed key so no Unauthorized calls (add this as const. parameter in service)


            //GoogleApi TestingJSONreslut = Newtonsoft.Json.JsonSerializer.Deserialize<GoogleApi>(result);
           


            Location UserLocation = new Location
            {
                Street = Registermodel.Street,
                HouseNumber = Registermodel.HouseNumber,
                City = Registermodel.City,
                Country = Registermodel.Country,
                //Latitude = testing.geometry.location.lat,
                //Longitude = testing.geometry.location.lng,
                Latitude = 1,
                Longitude = 1,
            };

            _locationRepository.Addlocation(UserLocation);

            var user = new ApplicationUser
            {
                UserName = Registermodel.Email,
                Email = Registermodel.Email,
                LocationId = UserLocation.LocationtId,                                                                                      
            };

         var resulting = await _userManager.CreateAsync(user, Registermodel.Password);
         var receivedUser = await _userManager.FindByEmailAsync(Registermodel.Email);
            await _userManager.AddToRoleAsync(receivedUser, "User");


            if (resulting.Succeeded)
            {
                return View();
            }

            return View(Registermodel);
        }

        [HttpPost]
        public async Task<IActionResult> ShelterRegisterAsync(ShelterRegisterViewModel Registermodel)
        {

           
            Location ShelterLocation = new Location
            {
                Street = Registermodel.Street,
                HouseNumber = Registermodel.HouseNumber,
                City = Registermodel.City,
                Country = Registermodel.Country,
                Latitude = 1,
                Longitude = 1
            };

            _locationRepository.Addlocation(ShelterLocation);

            var shelter = new Shelter
            {
                Name = Registermodel.Name,
                Email = Registermodel.Email,
                Description = Registermodel.Description,
                PhoneNumber = Registermodel.PhoneNumber,
                LocationId = ShelterLocation.LocationtId,
            };

            _shelterRepository.AddShelter(shelter);

            var user = new ApplicationUser
            {
                UserName = Registermodel.Email,
                Email = Registermodel.Email,
                ShelterId = shelter.ShelterId
   
            };

           
            var resulting = await _userManager.CreateAsync(user, Registermodel.Password);
            var receivedUser = await _userManager.FindByEmailAsync(Registermodel.Email);
            await _userManager.AddToRoleAsync(receivedUser, "Admin");


            if (resulting.Succeeded)
            {
                return View();
            }

            return View(Registermodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}