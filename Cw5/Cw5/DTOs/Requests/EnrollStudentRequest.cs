using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cw5.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        [Required]
        public string IndexNumber { set; get; }

        [Required]
        [MaxLength(10)]
        public string FirstName { set; get; }

        [Required]
        [MaxLength(255)]
        public string LastName { set; get; }

        [Required]
        public string BirthDate { set; get; }

        [Required]
        public string Studies { set; get; }
    }
}
