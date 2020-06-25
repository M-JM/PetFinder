using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.ViewModels
{
    public class PetCreateViewModel : Pet
    {
      
        public List<SelectListItem> Testing { get; private set; }
        public List<SelectListItem> PetKindList { get; set; }
        public IEnumerable<SelectListItem> PetColorList { get; set; }
        public IEnumerable<SelectListItem> PetRaceList { get; set; }
        public List<IFormFile> PetPictures { get; set; }

        public PetCreateViewModel(IEnumerable<PetColor> colors, IEnumerable<PetKind> petKinds, IEnumerable<PetRace> petRaces)
        {
            PetColorList = colors.Select(r => 
            new SelectListItem()
            {
                Value = r.PetColorId.ToString(),
                Text = r.Color
            }).ToList();

            PetRaceList = petRaces.Select(r =>
            new SelectListItem()
            {
                Value = r.PetRaceId.ToString(),
                Text = r.RaceName
            });

            PetKindList = petKinds.Select(r =>
            new SelectListItem()
            {
                Value = r.PetKindId.ToString(),
                Text = r.AnimalType
            }).ToList();

        }

    }
}
