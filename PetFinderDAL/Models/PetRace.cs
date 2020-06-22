using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PetFinderDAL.Models
{
  public  class PetRace
    {
        public int PetRaceId { get; set; }
        public string RaceName { get; set; }
        public List<Pet> Pets { get; set; }
    }
}
