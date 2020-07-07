﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PetFinderDAL.Models;

namespace PetFinder.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
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
    }
}