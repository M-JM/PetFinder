using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetFinderDAL.Context;
using PetFinderDAL.Models;

namespace PetFinder
{
    public class EditModel : PageModel
    {
        private readonly PetFinderDAL.Context.AppDbContext _context;

        public EditModel(PetFinderDAL.Context.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Pet Pet { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Pet = await _context.Pets
                .Include(p => p.PetColor)
                .Include(p => p.PetRace)
                .Include(p => p.Shelter).FirstOrDefaultAsync(m => m.PetId == id);

            if (Pet == null)
            {
                return NotFound();
            }
           ViewData["PetColorId"] = new SelectList(_context.PetColors, "PetColorId", "PetColorId");
           ViewData["PetRaceId"] = new SelectList(_context.PetRaces, "PetRaceId", "PetRaceId");
           ViewData["ShelterId"] = new SelectList(_context.Shelters, "ShelterId", "ShelterId");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Pet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetExists(Pet.PetId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PetExists(int id)
        {
            return _context.Pets.Any(e => e.PetId == id);
        }
    }
}
