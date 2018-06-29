using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestForSintezTech.ViewModels
{
    public class SimpleSubdivisionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public SimpleSubdivisionViewModel() { }
        public SimpleSubdivisionViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
