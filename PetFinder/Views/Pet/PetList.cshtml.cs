﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PetFinderDAL.Context;
using PetFinderDAL.Models;

namespace PetFinder
{
    public class IndexModel : PageModel
    {
        private readonly PetFinderDAL.Context.AppDbContext _context;

        public IndexModel(PetFinderDAL.Context.AppDbContext context)
        {
            _context = context;
        }

        public IList<Pet> Pet { get;set; }

        public async Task OnGetAsync()
        {
            Pet = await _context.Pets
                .Include(p => p.PetColor)
                .Include(p => p.PetRace)
                .Include(p => p.Shelter).ToListAsync();
        }
    }
}
