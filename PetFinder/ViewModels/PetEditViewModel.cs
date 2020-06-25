using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.ViewModels
{
    public class PetEditViewModel : PetCreateViewModel
    {
        public PetEditViewModel(IEnumerable<PetColor> colors, IEnumerable<PetKind> petKinds, IEnumerable<PetRace> petRaces) : base(colors, petKinds, petRaces)
        {
        }
    }
}
