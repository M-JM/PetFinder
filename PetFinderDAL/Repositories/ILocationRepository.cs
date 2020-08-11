using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetFinderDAL.Repositories
{
    public interface ILocationRepository
    {
        Location Addlocation(Location location);

        Location Updatelocation(Location location);

        Location GetLocation(int? id);
    }
}
