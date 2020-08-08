using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetFinderDAL.Models
{
    public class PetKind
    {
       
     
        public int PetKindId { get; set; }

        public string AnimalType { get; set; }

        public List<Pet> Pets { get; set; }

    }
}
