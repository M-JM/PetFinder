using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.ViewModels.AppointmentViewModel
{
    public class AppointmentCreateViewModel : Appointment
    {
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name="Date")]
        [Required(ErrorMessage ="Date is mandatory")]
        public new DateTime Date { get; set; }

       }
}
