using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw5.DAL;
using Cw5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{IndexNumber}")]
        public IActionResult GetStudent(string IndexNumber)
        {
            return Ok(_dbService.GetStudentEnrollments(IndexNumber));
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            // add to database
            // generate index number

            int indexNumber = new Random().Next(1, 20000);
            student.IndexNumber = $"s{indexNumber}";

            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id)
        {
            return Ok($"Aktualizacja dokończona, id studenta: {id}");
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveStudent(int id)
        {
            return Ok($"Usuwanie ukończone, id studenta: {id}");
        }
    }
}
