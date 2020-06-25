using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public PetCreateViewModel()
        {
         //https://stackoverflow.com/questions/54237069/model-bound-complex-types-must-not-be-abstract-or-value-types-and-must-have-a-pa
        }


        public List<SelectListItem> PetKindList { get; set; }
        public List<SelectListItem> PetColorList { get; set; }
        public List<SelectListItem> PetRaceList { get; set; }
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
            }).ToList();

            PetKindList = petKinds.Select(r =>
            new SelectListItem()
            {
                Value = r.PetKindId.ToString(),
                Text = r.AnimalType
            }).ToList();

        }

    }
}
