using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Models
{
    public class ExceptionDetail
    {
        public string ExceptionPath { get; set; }

        public string ExceptionMessage { get; set; }

        public string Stacktrace { get; set; }
    }
}
