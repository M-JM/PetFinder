using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.ViewModels.HomeViewModel
{
    public class AdminIndexViewModel
    {
        public IList<Appointment> appointments { get; set; }
        public IList<Pet> Pets { get; set; }
        public IList<ApplicationUser> Employees { get; set; }


        public int TotalPets { get { return Pets.Count(); } }

        public int TotalUsers { get { return Employees.Count(); } }

        public int TotalAppointmentPending { get { return appointments.Count(x => x.AppointmentStatus.StatusName == "Pending"); } }

        public int TotalAppointmentConfirmed { get { return appointments.Count(x => x.AppointmentStatus.StatusName == "Accepted"); } }

    }
}
