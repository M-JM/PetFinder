using PetFinderDAL.Models;
using System.Collections.Generic;
using System.Linq;


namespace PetFinderDAL.Repositories
{
   public interface IPetRepository
    {
        //Create

        Pet AddPet(Pet pet);
        PetPicture AddPetPicture(PetPicture petPicture);

        //Read
        IEnumerable<Pet> GetAllPets();
        List<PetColor> GetPetColors();
        List<PetRace> GetPetRaces();
        List<PetKind> GetPetKinds();
        Pet GetById(int id);

        IEnumerable<Pet> GetSearchedPets(SearchModel searchmodel);

        //Update

        //Delete

        Pet RemovePet(Pet pet);
        //PetPicture RemovePetPicture(PetPicture petPicture);
    }
}
