using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetFinderDAL.Context;
using PetFinderDAL.Models;

namespace PetFinder
{
    public class CreateModel : PageModel
    {
        private readonly PetFinderDAL.Context.AppDbContext _context;

        public CreateModel(PetFinderDAL.Context.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["PetColorId"] = new SelectList(_context.PetColors, "PetColorId", "PetColorId");
        ViewData["PetRaceId"] = new SelectList(_context.PetRaces, "PetRaceId", "PetRaceId");
        ViewData["ShelterId"] = new SelectList(_context.Shelters, "ShelterId", "ShelterId");
            return Page();
        }

        [BindProperty]
        public Pet Pet { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Pets.Add(Pet);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
