using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetFinderDAL.Models
{
    public class SearchModel
    {

        public List<int> PetColorId { get; set; }

        public List<int> PetRaceId { get; set; }

        public List<int?> PetKindId { get; set; }

        public Tristate[] SocialWithDogs { get; set; }

        public Tristate[] SocialWithCats { get; set; }

        public Tristate[] Appartmentfit { get; set; }

        public Tristate[] KidsFriendly { get; set; }

        public List<string> Size { get; set; }

        public IList<string> Gender { get; set; }

        public List<string> Age { get; set; }

    }

}

