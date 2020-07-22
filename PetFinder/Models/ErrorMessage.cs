using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Models
{
    public class ErrorMessage
    {
        public string TheErrorMessage { get; set; }

        public string Path { get; set; }

        public string Qs { get; set; }
    }
}
