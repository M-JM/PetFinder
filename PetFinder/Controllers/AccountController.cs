using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Core;
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
        private readonly IEmailService _emailService;
        private const string _baseUrl = "https://maps.googleapis.com/maps/api/geocode/json?address=a&key=";

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            ILocationRepository locationRepository,
            IShelterRepository shelterRepository,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _locationRepository = locationRepository;
            _shelterRepository = shelterRepository;
            _emailService = emailService;
        }

        private async Task<string> GetGeoAsync()
        {
            using HttpClient client = new HttpClient();
            HttpResponseMessage responseBody = await client.GetAsync(_baseUrl);
            var reply = await responseBody.Content.ReadAsStringAsync();
            return reply;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins =
                 (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                                new { ReturnUrl = returnUrl });
            var properties = _signInManager
                .ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins =
                        (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState
                    .AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Login", loginViewModel);
            }

            // Get the login information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState
                    .AddModelError(string.Empty, "Error loading external login information.");

                return View("Login", loginViewModel);
            }

            // If the user already has a login (i.e if there is a record in AspNetUserLogins
            // table) then sign-in the user with this external login provider
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            // If there is no record in AspNetUserLogins table, the user may not have
            // a local account
            else
            {
                // Get the email claim value
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    // Create a new user without password if we do not have a user already
                    var user = await _userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),

                        };

                        await _userManager.CreateAsync(user);
                        await _userManager.AddToRoleAsync(user, "User");
                    }

                    // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                // If we cannot find the user email we cannot continue
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on PetfinderInfo@gmail.com";

                return View("Error");
            }
        }

        [HttpPost]
        [AllowAnonymous]
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
                    HttpContext.Session.SetString("Username", currentuser.Email);
                    HttpContext.Session.SetString("id", currentuser.Id);

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
        [AllowAnonymous]
        public IActionResult Register()
        {
            UserRegisterViewModel Registermodel = new UserRegisterViewModel
            {
            };

            return View(Registermodel);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ShelterRegister()
        {
            ShelterRegisterViewModel Registermodel = new ShelterRegisterViewModel
            {
            };

            return View(Registermodel);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync(UserRegisterViewModel Registermodel)
        {

            using HttpClient client = new HttpClient();

            //WORKING CODE
            //var testingasstring= await client.GetAsync(_baseUrl);
            //var result = testingasstring.Content.ReadAsStringAsync().Result;
            //GoogleApi.Rootobject google = JsonConvert.DeserializeObject<GoogleApi.Rootobject>(result);
            //var testing = google.results.FirstOrDefault();
            // ENDWORKING CODE

            //Seperate document ->store API key local ( no push to Git) .

            //var testingasJson = await client.GetFromJsonAsync<GoogleApi>(_baseUrl);
            //var test = await testingasstring.Content.ReadAsStringAsync();

            // Entire API call to google needs to be rewritten into a service with method accepting Concatnated Address parameter
            // In case the address return no object -> use City ZIP CODE to get at least approx. geocoding
            // Update the Location class with ZIP CODE property
            // Temporarly removed key so no Unauthorized calls (add this as const. parameter in service)


            //GoogleApi TestingJSONreslut = Newtonsoft.Json.JsonSerializer.Deserialize<GoogleApi>(result);



            Location location = AddLocation(Registermodel);
            int? shelter = null;

            ApplicationUser user = AddUser(Registermodel, location, shelter);

            IdentityResult resulting = await _userManager.CreateAsync(user, Registermodel.Password);
            ApplicationUser receivedUser = await _userManager.FindByEmailAsync(Registermodel.Email);
            await _userManager.AddToRoleAsync(receivedUser, "User");


            if (resulting.Succeeded)
            {
                // gen email conf.
                string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string link = Url.Action(nameof(VerifyEmail), "Account", new { userId = user.Id, code }, Request.Scheme, Request.Host.ToString());

               await _emailService.SendAsync(user.NormalizedEmail,"Email Verify",$"<a href=\"{link}\"> Verify your Email </a>",true);

                return View("Index", "Home");
            }

            return View(Registermodel);
        }
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(string userId, string code)
        {
            var user =  await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded) {

                return View("VerifyYourEmail", "Account");
            }
            return View("Index", "Home");

        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminRegister(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Admin"))
                {
                    ApplicationUser admin = await _userManager.FindByEmailAsync(User.Identity.Name);
                    Shelter shelter = _shelterRepository.GetShelterById(admin.ShelterId);
                    Location location = AddLocation(model);

                    ApplicationUser user = AddUser(model, location, shelter.ShelterId);

                    IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                    ApplicationUser receivedUser = await _userManager.FindByEmailAsync(model.Email);
                    await _userManager.AddToRoleAsync(receivedUser, "ShelterUser");
                    // If user is successfully created
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }
                    // If there are any errors, add them to the ModelState object
                    // which will be displayed by the validation summary tag helper

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                return View(model);
            }
            return View("Error"); // global expection
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ShelterRegisterAsync(ShelterRegisterViewModel Registermodel)
        {

            Location location = AddLocation(Registermodel);

            Shelter shelter = new Shelter
            {
                Name = Registermodel.Name,
                Email = Registermodel.Email,
                Description = Registermodel.Description,
                PhoneNumber = Registermodel.PhoneNumber,
                LocationId = location.LocationtId,
            };

            _shelterRepository.AddShelter(shelter);

            ApplicationUser user = AddUser(Registermodel, location, shelter.ShelterId);

            IdentityResult resulting = await _userManager.CreateAsync(user, Registermodel.Password);
            ApplicationUser receivedUser = await _userManager.FindByEmailAsync(Registermodel.Email);
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
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ToDo
        // Make generic Viewmodel Interface for Location and user to make a Method who takes Generic Viewmodel to perform 
        // Location and user creation - DRY in both ShelterReg. and UserReg.

        public Location AddLocation(RegisterViewModel model)
        {
            Location Location = new Location
            {
                Street = model.Street,
                HouseNumber = model.HouseNumber,
                City = model.City,
                Country = model.Country,
                Zipcode = model.Zipcode,
                Latitude = 1,
                Longitude = 1
            };
            _locationRepository.Addlocation(Location);

            return Location;
        }

        public ApplicationUser AddUser(RegisterViewModel model, Location location, int? shelterid)
        {

            ApplicationUser user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                LocationId = location.LocationtId,
                ShelterId = shelterid

            };

            return user;
        }


    }
}