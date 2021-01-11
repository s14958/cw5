using Cw4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw4.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
        public ICollection<string> GetStudentEnrollments(string IndexNumber);
    }
}
