using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cw5.DTOs.Requests
{
    public class PromoteStudentsRequest
    {
        [Required]
        public string Studies { set; get; }

        [Required]
        public int Semester { set; get; }
    }
}
