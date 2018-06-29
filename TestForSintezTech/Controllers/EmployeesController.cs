using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestForSintezTech.Models;
using TestForSintezTech.ViewModels;

namespace TestForSintezTech.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class EmployeesController : Controller
    {
        private ApplicationContext db;
        public EmployeesController(ApplicationContext context)
        {
            db = context;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var employees = await db.Employees.ToListAsync();
            var result = JsonConvert.SerializeObject(employees, new JsonSerializerSettings { Formatting = Formatting.Indented });

            return Content(result);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{subdivisionId}")]
        public async Task<IActionResult> Get(int subdivisionId)
        {
            //var employees = db.Employees
            //    .Include(p => p.Positions)
            //    .ThenInclude(pt => pt.Subdivision)
            //    .Where(e => e.Positions.Any(x => x.Subdivision.Id == subdivisionId));


            await db.Employees.ForEachAsync(e =>
            {
                db.Entry(e).Collection(p => p.Positions).Load();
                e.Positions.ForEach(p => db.Entry(p).Reference(s => s.Subdivision).Load());
            });
            var employees = db.Employees.Where(e => e.Positions.Any(x => x.Subdivision.Id == subdivisionId));


            var employeeVms = new List<EmployeeViewModel>(employees.Count());
            foreach (var e in employees)
            {
                e.Positions.RemoveAll(p => p.Subdivision.Id != subdivisionId);

                employeeVms.Add(new EmployeeViewModel(e));
            }

            return Content(JsonConvert.SerializeObject(employeeVms.ToList(), new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]EmployeeViewModel param)
        {
            if (param == null)
                return BadRequest();

            var subdivision = db.Subdivisions.First(x => x.Id == param.Subdivision);
            var employee = param.ToEmployee(subdivision);

            db.Employees.Add(employee);
            await db.SaveChangesAsync();

            return Ok();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]EmployeeViewModel employeeVm)
        {
            if (employeeVm == null)
                return BadRequest();

            if (!db.Employees.Any(u => u.Id == id))
                return NotFound();

            var employee = new Employee
            {
                Id = employeeVm.Id,
                FirstName = employeeVm.FirstName,
                LastName = employeeVm.LastName,
                Patronymic = employeeVm.Patronymic,
                Age = employeeVm.Age,
                Gender = employeeVm.Gender,
                Positions = new List<Position>()
            };

            var subdivision = db.Subdivisions.First(x => x.Id == employeeVm.Subdivision);

            employee.Positions.Add(new Position
            {
                Subdivision = subdivision,
                PositionName = employeeVm.Position
            });

            db.Update(employee);
            await db.SaveChangesAsync();
            return Ok();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("employeeId={employeeId}&subdivisionId={subdivisionId}")]
        public async Task<IActionResult> Delete(int employeeId, int subdivisionId)
        {
            var user = db.Employees.FirstOrDefault(u => u.Id == employeeId);
            if (user == null)
                return NotFound();

            db.Entry(user).Collection(u => u.Positions).Load();
            user.Positions.ForEach(p => db.Entry(p).Reference(s => s.Subdivision).Load());

            user.Positions.RemoveAll(p => p.Subdivision?.Id == subdivisionId);

            if (user.Positions.Count == 0)
                db.Employees.Remove(user);

            await db.SaveChangesAsync();

            return Ok();
        }
    }
}
