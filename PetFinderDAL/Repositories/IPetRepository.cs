using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetFinderDAL.Repositories
{
   public interface IPetRepository
    {
        //Create

        Pet AddPet(Pet pet);
        PetPicture AddPetPicture(PetPicture petPicture);

        //Read

        IEnumerable<PetColor> GetPetColors();
        IEnumerable<PetRace> GetPetRaces();
        IEnumerable<PetKind> GetPetKinds();
        Pet GetById(int id);

        //Update

        //Delete

        Pet RemovePet(Pet pet);
        //PetPicture RemovePetPicture(PetPicture petPicture);
    }
}
