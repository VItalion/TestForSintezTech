using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestForSintezTech.Models
{
    public class Subdivision
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Employee> Employees { get; set; }
        public List<Subdivision> Children { get; set; }
        public Subdivision Parent { get; set; }
    }
}
