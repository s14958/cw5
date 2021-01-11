using Cw4.DAL;
using Cw4.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Cw4.DAL
{
    public class DbService : IDbService
    {
        private static string connectionString = "Data Source=db-mssql;Initial Catalog=s14958;Integrated Security=True";

        public IEnumerable<Student> GetStudents()
        {
            List<Student> students = new List<Student>();

            using (var client = new SqlConnection(connectionString))
                using(var command = new SqlCommand())
            {
                command.Connection = client;
                command.CommandText = "select FirstName, LastName, IndexNumber, BirthDate, Studies.Name, Enrollment.Semester from Student " +
                    "left join Enrollment on Student.IdEnrollment = Enrollment.IdEnrollment " +
                    "left join Studies on Enrollment.IdStudy = Studies.IdStudy";
                client.Open();

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var student = new Student
                    {
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        IndexNumber = reader["IndexNumber"].ToString(),
                        BirthDate = DateTime.Parse(reader["BirthDate"].ToString()),
                        StudyName = reader["Name"].ToString()
                    };

                    students.Add(student);
                }
            }

            return students;
        }

        public ICollection<string> GetStudentEnrollments(string IndexNumber)
        {
            ICollection<string> studies = new List<string>();
            using (var client = new SqlConnection(connectionString))
                using(var command = new SqlCommand())
            {
                command.Connection = client;
                command.CommandText = "select Studies.Name from Student " +
                    "left join Enrollment on Student.IdEnrollment = Enrollment.IdEnrollment " +
                    "left join Studies on Enrollment.IdStudy = Studies.IdStudy " +
                    $"where IndexNumber = @IndexNumber;";
                command.Parameters.AddWithValue("IndexNumber", IndexNumber);

                client.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    studies.Add(reader["Name"].ToString());
                }
            }

            return studies;
        }
    }
}
