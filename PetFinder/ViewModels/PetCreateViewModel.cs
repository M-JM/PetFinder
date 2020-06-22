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
        public IEnumerable<SelectListItem> PetKindList { get; set; }
        public IEnumerable<SelectListItem> PetColorList { get; set; }
        public IEnumerable<SelectListItem> PetRaceList { get; set; }
        public List<IFormFile> PetPictures { get; set; }

    }
}
