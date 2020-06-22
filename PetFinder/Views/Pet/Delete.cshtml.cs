using System;
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
    public class DeleteModel : PageModel
    {
        private readonly PetFinderDAL.Context.AppDbContext _context;

        public DeleteModel(PetFinderDAL.Context.AppDbContext context)
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Pet = await _context.Pets.FindAsync(id);

            if (Pet != null)
            {
                _context.Pets.Remove(Pet);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
