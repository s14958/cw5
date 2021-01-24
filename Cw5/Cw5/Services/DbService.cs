using Cw5.DAL;
using Cw5.DTOs.Requests;
using Cw5.DTOs.Responses;
using Cw5.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Cw5.DAL
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
                        Studies = reader["Name"].ToString()
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

        public Enrollment EnrollStudent(EnrollStudentRequest request)
        {
            using (var connection = new SqlConnection(connectionString))
                using(var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();
                var transaction = connection.BeginTransaction();
                command.Transaction = transaction;

                try
                {
                    var studyId = FindStudiesByName(request.Studies, command);
                  
                    command.CommandText = "select * from Enrollment where IdStudy = @idStudy and Semester = 1";
                    command.Parameters.AddWithValue("idStudy", studyId);

                    var reader = command.ExecuteReader();
                    var enrollment = new Enrollment();
                    int idEnrollment;
                    // Add Enrollemnt if it doesn't exist
                    if (!reader.Read())
                    {
                        enrollment = new Enrollment
                        {
                            IdStudy = studyId,
                            Semester = 1,
                            StartDate = DateTime.Now
                        };

                        reader.Close();
                        command.Parameters.Clear();
                        command.CommandText = "insert into Enrollment output INSERTED.IdEnrollment values(1, @idStudy, @startDate)";

                        command.Parameters.Add("IdEnrollemnt", System.Data.SqlDbType.Int, 4).Direction = System.Data.ParameterDirection.Output;
                        command.Parameters.AddWithValue("idStudy", enrollment.IdStudy);
                        command.Parameters.AddWithValue("startDate", enrollment.StartDate);
                        enrollment.IdEnrollment = (int)command.ExecuteScalar();
                    }
                    else
                    {
                        idEnrollment = (int)reader["IdEnrollment"];
                        enrollment.IdEnrollment = (int)reader["IdEnrollment"];
                        enrollment.Semester = (int)reader["Semester"];
                        enrollment.IdStudy = (int)reader["IdStudy"];
                        enrollment.StartDate = DateTime.Parse(reader["StartDate"].ToString());
                    }

                    // Add Student
                    reader.Close();
                    command.Parameters.Clear();

                    var birthDate = DateTime.ParseExact(request.BirthDate, "dd.MM.yyyy", null);

                    command.CommandText = "insert into Student(FirstName, LastName, IndexNumber, BirthDate, IdEnrollment) values(@firstName, @lastName, @indexNumber, @birthDate, @idEnrollment); ";
                    command.Parameters.AddWithValue("firstName", request.FirstName);
                    command.Parameters.AddWithValue("lastName", request.LastName);
                    command.Parameters.AddWithValue("indexNumber", request.IndexNumber);
                    command.Parameters.AddWithValue("birthDate", birthDate);
                    command.Parameters.AddWithValue("idEnrollment", enrollment.IdEnrollment);

                    command.ExecuteNonQuery();

                    transaction.Commit();

                    return enrollment;
                } catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception($"Error while writing data to DB: {e.Message}");
                }
            }
        }

        public Enrollment PromoteStudents(int semester, string studies)
        {
            using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();
                var transaction = connection.BeginTransaction();
                command.Transaction = transaction;

                try
                {
                    int studyId = FindStudiesByName(studies, command);
                    int enrollmentId = GetEnrollment(semester, studyId, command);

                    command.CommandText = "update Enrollment set Semester += 1 where Semester = @semester and IdStudy = @studyId;";
                    command.Parameters.AddWithValue("semester", semester);
                    command.Parameters.AddWithValue("studyId", studyId);

                    command.ExecuteNonQuery();

                    var enrollment = GetEnrollment(enrollmentId, command);

                    transaction.Commit();

                    return enrollment;
                } catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception($"Error while writing data to DB: {e.Message}");
                }
            }
        }

        private int FindStudiesByName(string studiesName, SqlCommand command)
        {
            command.CommandText = "select * from Studies where name = @studies";
            command.Parameters.AddWithValue("studies", studiesName);
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                throw new Exception("Nie ma takich studiów");
            }

            var studyId = (int)reader["IdStudy"];
            ClearCommand(command, reader);

            return studyId;
        }

        // zwraca ID pierwszego istniejącego enrollmentu (ale zakładam, że to jest akceptowalne)
        private int GetEnrollment(int semester, int studyId, SqlCommand command)
        {
            command.CommandText = "select * from Enrollment where Semester = @semester and IdStudy = @studyId;";
            command.Parameters.AddWithValue("semester", semester);
            command.Parameters.AddWithValue("studyId", studyId);

            var reader = command.ExecuteReader();
            
            bool isData = reader.Read();

            if (!isData)
            {
                reader.Close();
                throw new Exception($"Nie istnieje wpis dot semestru {semester} na studiach od ID: {studyId}");
            }

            var enrollmentId = (int)reader["IdEnrollment"];
            ClearCommand(command, reader);
            return enrollmentId;
        }

        private Enrollment GetEnrollment(int enrollmentId, SqlCommand command)
        {
            command.CommandText = "select * from Enrollment where IdEnrollment = @enrollmentId;";
            command.Parameters.AddWithValue("enrollmentId", enrollmentId);

            var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                throw new Exception($"Nie ma Enrollmentu o id: {enrollmentId}");
            }

            var enrollment = new Enrollment
            {
                IdEnrollment = enrollmentId,
                IdStudy = (int)reader["IdStudy"],
                Semester = (int)reader["Semester"],
                StartDate = DateTime.Parse(reader["StartDate"].ToString())
            };

            ClearCommand(command, reader);

            return enrollment;
        }

        private void ClearCommand(SqlCommand command, SqlDataReader reader)
        {
            reader.Close();
            command.CommandText = null;
            command.Parameters.Clear();
        }
    }
}
