using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.ViewModels
{
    public class PetDetailViewModel 
    {
        public Pet Pet { get; set; }

        public bool Isfavorite { get; set; }

        public string Age { get; set; }
    }
}
