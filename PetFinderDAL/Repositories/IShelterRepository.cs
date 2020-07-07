using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetFinderDAL.Repositories
{
   public interface IShelterRepository
    {
        Shelter AddShelter(Shelter Shelter);
        Shelter GetShelterById(int? id);
    }
}
