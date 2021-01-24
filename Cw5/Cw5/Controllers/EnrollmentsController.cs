using Cw5.DAL;
using Cw5.DTOs.Requests;
using Cw5.DTOs.Responses;
using Cw5.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw5.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : Controller
    {
        private readonly IDbService _dbService;

        public EnrollmentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetEnrollments()
        {
            return Ok("List of enrollments");
        }

        [HttpPost]
        public IActionResult CreateEnrollment(EnrollStudentRequest student)
        {
            try
            {
                var enrollment = _dbService.EnrollStudent(student);

                return Ok(new EnrollStudentResponse { 
                    LastName = student.LastName,
                    Semester = enrollment.Semester,
                    StartDate = enrollment.StartDate
                });
            } catch (Exception e)
            {
                return BadRequest($"Błąd: {e.Message}");
            }
        }

        [HttpPost]
        [Route("promotions")]
        public IActionResult PromoteAll(PromoteStudentsRequest promoteData)
        {
            try
            {
                Enrollment enrollment = _dbService.PromoteStudents(promoteData.Semester, promoteData.Studies);
                return Ok(enrollment);
            } catch (Exception e)
            {
                return BadRequest($"Błąd: {e.Message}");
            }
        }
    }
}
