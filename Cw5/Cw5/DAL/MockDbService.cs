using Cw4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw4.DAL
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;
        private static ICollection<string> _enrollments;

        static MockDbService()
        {
            _students = new List<Student> {
                new Student { IdStudent=1, FirstName="Jan", LastName="Kowalski" },
                new Student { IdStudent=2, FirstName="Anna", LastName="Malewski" },
                new Student { IdStudent=3, FirstName="Andrzej", LastName="Andrzejewicz" }
            };

            _enrollments = new List<string> { "Matematyka", "Informatyka" };
        }

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }

        public ICollection<string> GetStudentEnrollments(string IndexNumber)
        {
            return _enrollments;
        }
    }
}
