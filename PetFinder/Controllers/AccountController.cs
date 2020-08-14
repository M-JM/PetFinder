using System.Linq;
using System.Net.Http;
using System.Security.Claims;
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
        private const string _baseUrl = "https://maps.googleapis.com/maps/api/geocode/json?address=";
        private const string _apiKey = "&key=AIzaSyCr0pt_xuJO0N8VFX_OB2iV6nOlee9_d1I";

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
            returnUrl ??= Url.Content("~/");

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

            
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState
                    .AddModelError(string.Empty, "Error loading external login information.");

                return View("Login", loginViewModel);
            }

            
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            
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
                var currentuser = await _userManager.FindByEmailAsync(model.Email);
              
                if(currentuser != null)
                {
                    if (currentuser.EmailConfirmed == true)
                    {
                        if (signInResult.Succeeded)
                        {
                            if (string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            HttpContext.Session.SetString("Username", currentuser.UserName);
                            HttpContext.Session.SetString("id", currentuser.Id);
                            HttpContext.Session.SetString("shelterid", currentuser.ShelterId.ToString());

                            if (await _userManager.IsInRoleAsync(currentuser, "Admin") && currentuser.EmailConfirmed == true)
                            {
                                return RedirectToAction("AdminIndex", "Home");
                            }
                            else if (await _userManager.IsInRoleAsync(currentuser, "User") && currentuser.EmailConfirmed == true)
                            {
                                return RedirectToAction("Index", "Home");
                            }
                         return RedirectToAction("Index", "Home");
                        }
                        model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                        ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                        return View(model);
                    }
                    return View("Index", "Account");
                }
                return RedirectToAction("Register", "Account");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync()
        {
            UserRegisterViewModel Registermodel = new UserRegisterViewModel
            {
                ExternalLogins =
                 (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
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

       
            if (ModelState.IsValid) {

               

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
            Registermodel.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
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
                    await _userManager.AddToRoleAsync(receivedUser, "Admin");
                    // If user is successfully created
                    if (result.Succeeded)
                    {
                        return RedirectToAction("AdminIndex", "Home");
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
                string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string link = Url.Action(nameof(VerifyEmail), "Account", new { userId = user.Id, code }, Request.Scheme, Request.Host.ToString());

                await _emailService.SendAsync(user.NormalizedEmail, "Email Verify", $"<a href=\"{link}\"> Verify your Email </a>", true);

                return View("Index", "Home");
            }

            return View(Registermodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task<GoogleApi.Result> GetGeoAsync(string address)
        {
            using HttpClient client = new HttpClient();
            string response = _baseUrl + address + _apiKey;
            HttpResponseMessage responseBody = await client.GetAsync(response);
            string result = responseBody.Content.ReadAsStringAsync().Result;
            GoogleApi.Rootobject google = JsonConvert.DeserializeObject<GoogleApi.Rootobject>(result);
            GoogleApi.Result apiCallResult = google.results.FirstOrDefault();

            return apiCallResult;

        }

        private Location AddLocation(RegisterViewModel model)
        {
            string address = model.Street +" " + model.HouseNumber +" "+ model.Zipcode +" " + model.Country;
            Task<GoogleApi.Result> googleApiResult = GetGeoAsync(address);

            Location Location = new Location
            {
                Street = model.Street,
                HouseNumber = model.HouseNumber,
                City = model.City,
                Country = model.Country,
                Zipcode = model.Zipcode,
                Latitude = googleApiResult.Result.geometry.location.lat,
                Longitude = googleApiResult.Result.geometry.location.lng,
            };
            _locationRepository.Addlocation(Location);

            return Location;
        }

        private ApplicationUser AddUser(RegisterViewModel model, Location location, int? shelterid)
        {

            ApplicationUser user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                LocationId = location.LocationtId,
                ShelterId = shelterid,
                LastName = model.LastName,
                FirstName = model.FirstName,
                PhoneNumber = model.PhoneNumber,
                
            };

            return user;
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Profile(string id)
        {
            Location location;

            ApplicationUser user = await _userManager.FindByNameAsync(id);

            if(user.LocationId == null) {
                location = new Location()
                {
                    City = "",
                    Street = "",
                    Country = "",
                    HouseNumber = 0,
                };
            }
            else
            {
            location = _locationRepository.GetLocation(user.LocationId);
            }


            UserProfileViewModel model = new UserProfileViewModel
            {
               Id = user.Id,
               LastName = user.LastName,
               FirstName = user.FirstName,
               Email = user.UserName,
               Street = location.Street,
               City = location.City,
               Country = location.Country,
               Zipcode = location.Zipcode,
               HouseNumber = location.HouseNumber,
               PhoneNumber = user.PhoneNumber

               
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByNameAsync(model.Id);
                Location location = _locationRepository.GetLocation(user.LocationId);

                string address = model.Street + " " + model.HouseNumber + " " + model.Zipcode + " " + model.Country;
                Task<GoogleApi.Result> googleApiResult = GetGeoAsync(address);

                if (user == null)
                {
                    return View("NotFound");
                }

                         
                if(location == null)
                {
                    location = new Location
                    {
                        Street = model.Street,
                        HouseNumber = model.HouseNumber,
                        City = model.City,
                        Country = model.Country,
                        Zipcode = model.Zipcode,
                        Latitude = googleApiResult.Result.geometry.location.lat,
                        Longitude = googleApiResult.Result.geometry.location.lng,
                    };
                    _locationRepository.Addlocation(location);
                }
                else
                {
                    location.Street = model.Street;
                    location.Zipcode = model.Zipcode;
                    location.HouseNumber = model.HouseNumber;
                    location.Country = model.Country;
                    location.Latitude = googleApiResult.Result.geometry.location.lat;
                    location.Longitude = googleApiResult.Result.geometry.location.lng;

                    _locationRepository.Updatelocation(location);
                };

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Id;
                user.PhoneNumber = model.PhoneNumber;
                user.LocationId = location.LocationtId;


                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded && User.IsInRole("User"))
                {
                    return RedirectToAction("Index","Home") ;

                } else if (result.Succeeded && User.IsInRole("Admin"))
                {
                    return RedirectToAction("AdminIndex", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            ForgotPasswordViewModel model = new ForgotPasswordViewModel()
            {

            };

            return View(model);

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
              
                    string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    string link = Url.Action("ResetPassword", "Account", new { email = user.UserName, code }, Request.Scheme, Request.Host.ToString());

                    await _emailService.SendAsync(user.UserName, "Password Reset", $"<a href=\"{link}\"> Please follow this link to reset password </a>", true);

                    return View("PasswordForgotConf");

                }
                return View("PasswordForgotConf");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code, string email)
        {
            if (code == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            ResetPasswordViewModel model = new ResetPasswordViewModel
            {
                code = code,
                Email = email,
            };

            return View(model);

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.code, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);

                }
                return View("ResetPasswordConfirmation");
            }
            return View(model);
        }

    }
}