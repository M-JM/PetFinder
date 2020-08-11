using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.ViewModels.PetViewModel
{
   
    public class PetEditViewModel : PetCreateViewModel
    {
        public PetEditViewModel()
        {
          
        }

        public PetEditViewModel(IEnumerable<PetColor> colors, IEnumerable<PetKind> petKinds, IEnumerable<PetRace> petRaces) : base(colors, petKinds, petRaces)
        {

        }
    }
}
