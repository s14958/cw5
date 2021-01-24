using Cw5.DTOs.Requests;
using Cw5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw5.DAL
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

        public Enrollment EnrollStudent(EnrollStudentRequest request)
        {
            // student enrolled
            
            return new Enrollment
            {
                IdEnrollment = 1,
                IdStudy = 1,
                Semester = 1,
                StartDate = new DateTime()
            };
        }

        public Enrollment PromoteStudents(int semester, string studies)
        {
            // student promoted

            return new Enrollment
            {
                IdEnrollment = 1,
                IdStudy = 1,
                Semester = 2,
                StartDate = new DateTime()
            };
        }
    }
}
