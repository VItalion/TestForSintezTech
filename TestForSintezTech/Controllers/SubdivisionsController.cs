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
    public class SubdivisionsController : Controller
    {
        ApplicationContext db;
        public SubdivisionsController(ApplicationContext context)
        {
            db = context;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public IActionResult Get()
        {
            var subdivisions = db.Subdivisions.Where(s => s.Parent == null);
            var resultJson = JsonConvert.SerializeObject(subdivisions.ToList(), new JsonSerializerSettings { Formatting = Formatting.Indented });

            return Content(resultJson);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var subdivision = await db.Subdivisions.FirstOrDefaultAsync(s => s.Id == id);
            if (subdivision == null)
                return Content(JsonConvert.SerializeObject(null, new JsonSerializerSettings { Formatting = Formatting.Indented }));

            db.Entry(subdivision).Collection(s => s.Children).Load();
            var vm = new SubdivisionViewModel(subdivision);

            return Content(JsonConvert.SerializeObject(vm, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]SubdivisionViewModel vm)
        {
            if (vm == null)
                return BadRequest();

            Subdivision parent = null;
            List<Subdivision> children = null;

            if (vm.Parent != null)
                parent = await db.Subdivisions.FirstOrDefaultAsync(s => s.Id == vm.Parent.Id);

            if (vm.Children != null)
                children = db.Subdivisions.Where(s => vm.Children.Any(c => c.Id == s.Id)).ToList();

            var subdivision = vm.ToSubdivision(parent, children);

            db.Subdivisions.Add(subdivision);
            await db.SaveChangesAsync();

            return Ok();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]SubdivisionViewModel vm)
        {
            if (vm == null)
                return BadRequest();

            if (!db.Employees.Any(u => u.Id == id))
                return NotFound();

            var parent = await db.Subdivisions.FirstOrDefaultAsync(s => s.Id == vm.Parent.Id);
            var children = db.Subdivisions.Where(s => vm.Children.Any(c => c.Id == s.Id)).ToList();
            var subdivision = vm.ToSubdivision(parent, children);

            db.Update(subdivision);
            await db.SaveChangesAsync();
            return Ok();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var subdivision = db.Subdivisions.FirstOrDefault(s => s.Id == id);
            if (subdivision == null)
                return NotFound();

            db.Subdivisions.Remove(subdivision);
            await db.SaveChangesAsync();
            return Ok();
        }
    }
}