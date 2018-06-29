using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestForSintezTech.Models;

namespace TestForSintezTech.ViewModels
{
    public class SubdivisionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<SimpleSubdivisionViewModel> Children { get; set; }
        public SimpleSubdivisionViewModel Parent { get; set; }

        public SubdivisionViewModel() { }
        public SubdivisionViewModel(Subdivision subdivision)
        {
            Id = subdivision.Id;
            Name = subdivision.Name;

            Children = subdivision.Children?.Select(s => new SimpleSubdivisionViewModel { Name = s.Name, Id = s.Id }).ToList();
            if (subdivision.Parent != null)
                Parent = new SimpleSubdivisionViewModel(subdivision.Parent.Id, subdivision.Parent.Name);
        }

        public Subdivision ToSubdivision()
        {
            return new Subdivision()
            {
                Id = Id,
                Name = Name
            };
        }

        public Subdivision ToSubdivision(Subdivision parent, List<Subdivision> children)
        {
            var subdivision = this.ToSubdivision();
            subdivision.Parent = parent;
            subdivision.Children = children?.ToList();

            return subdivision;
        }
    }
}
