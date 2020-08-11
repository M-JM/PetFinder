using Microsoft.AspNetCore.Mvc.Rendering;
using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.ViewModels.PetViewModel
{
    public class SearchViewModel : Pet
    {
     
        public SearchViewModel()
        {

        }

        public List<SelectListItem> Genderlist { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "M", Text = "Male" },
            new SelectListItem { Value = "F", Text = "Female" },
        };
        public List<SelectListItem> SizeList { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Small", Text = "Small" },
            new SelectListItem { Value = "Medium", Text = "Medium" },
            new SelectListItem { Value = "Large", Text = "Large" },
        };

        public List<string> SizeListSearch { get; set; }

        public Tristate[] SocialWithDogs { get; set; }
        public Tristate[] Appartmentfit { get; set; }
        public Tristate[] SocialWithCats { get; set; }
        public Tristate[] KidsFriendly { get; set; }

        public string Age { get; set; }

        public int[] PetKindIdfromForm { get; set; }

        public List<SelectListItem> PetKindList { get; set; }
        public List<SelectListItem> PetColorList { get; set; }
        public List<SelectListItem> PetRaceList { get; set; }

        public SearchViewModel(IEnumerable<PetColor> colors, IEnumerable<PetKind> petKinds, IEnumerable<PetRace> petRaces)
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

        public IEnumerable<Pet> ListOfPets { get; set; }

    }
}
