using Microsoft.AspNetCore.Mvc.Rendering;
using PetFinderDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.ViewModels.AppointmentViewModel
{
    public class StatusViewModel
    {
        public StatusViewModel()
        {
            //https://stackoverflow.com/questions/54237069/model-bound-complex-types-must-not-be-abstract-or-value-types-and-must-have-a-pa
        }

        public int AppointmentStatusId { get; set; }

        public List<SelectListItem> ListStatuses { get; set; }
  
        public StatusViewModel(List<AppointmentStatus> statuses)
        {
            ListStatuses = statuses.Select(r =>
            new SelectListItem()
            {
                Value = r.AppointmentStatusId.ToString(),
                Text = r.StatusName
            }).ToList();



        }
    }
}

