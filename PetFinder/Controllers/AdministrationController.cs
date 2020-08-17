using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetFinderDAL.Models;

namespace PetFinder.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdministrationController> _logger;

        public AdministrationController(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AdministrationController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListUsersAsync()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            var users = new List<ApplicationUser>();

            if (User.IsInRole("Admin"))
            {
                users = _userManager.Users.Where(x => x.ShelterId == user.ShelterId).ToList();
            }
          
            return View(users);
        }

        
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    return View("NotFound");
                }

                var identityResult = await _userManager.DeleteAsync(user);

                if (identityResult.Succeeded)
                {
                    return RedirectToAction("AdminIndex", "Home");
                }
               

                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return RedirectToAction("AdminIndex", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"When deleting an user.");

                return View("Error");
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }

    


}