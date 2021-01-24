using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw5.Models
{
    public class Student
    {
        public int IdStudent { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string IndexNumber { set; get; }
        public DateTime BirthDate { set; get; }
        public string Studies { set; get; }
        public int Semester { set; get; }
    }
}
