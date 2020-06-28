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
        private const string _baseUrl = "https://maps.googleapis.com/maps/api/geocode/json?address=a&key=";

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            ILocationRepository locationRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _locationRepository = locationRepository;
        }

        private async Task<string> GetGeoAsync()
        {
            using HttpClient client = new HttpClient();
            HttpResponseMessage responseBody = await client.GetAsync(_baseUrl);
            var reply = await responseBody.Content.ReadAsStringAsync();
            return reply;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            UserRegisterViewModel Registermodel = new UserRegisterViewModel
            {
               
             
            };
            
            return View(Registermodel);
        }
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(UserRegisterViewModel Registermodel)
        {
         
            using HttpClient client = new HttpClient();
            var testingasJson = await client.GetFromJsonAsync<GoogleApi>(_baseUrl);
            var testingasstring= await client.GetAsync(_baseUrl);
            var result = testingasstring.Content.ReadAsStringAsync().Result;
            var test = await testingasstring.Content.ReadAsStringAsync();
            GoogleApi.Rootobject google = JsonConvert.DeserializeObject<GoogleApi.Rootobject>(result);
            var testing = google.results.FirstOrDefault();

            // Entire API call to google needs to be rewritten into a service with method accepting Concatnated Address parameter
            // In case the address return no object -> use City ZIP CODE to get at least approx. geocoding
            // Update the Location class with ZIP CODE property
            // Temporarly removed key so no Unauthorized calls (add this as const. parameter in service)


            //GoogleApi TestingJSONreslut = Newtonsoft.Json.JsonSerializer.Deserialize<GoogleApi>(result);
            System.Diagnostics.Debug.WriteLine(testingasstring.ToString());

            Location UserLocation = new Location
            {
                Street = Registermodel.Street,
                HouseNumber = Registermodel.HouseNumber,
                City = Registermodel.City,
                Country = Registermodel.Country,
                Latitude = testing.geometry.location.lat,
                Longitude = testing.geometry.location.lng,
            };

            _locationRepository.Addlocation(UserLocation);

            var user = new ApplicationUser
            {
                UserName = Registermodel.Email,
                Email = Registermodel.Email,
                LocationId = UserLocation.LocationtId
               
                                                                        
            };
         var resulting = await _userManager.CreateAsync(user, Registermodel.Password);
         

            if (resulting.Succeeded)
            {
                return View();
            }

            return View(Registermodel);
        }
        
    }
}