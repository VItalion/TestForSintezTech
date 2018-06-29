using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestForSintezTech.Models
{
    public class Position
    {
        [Key]
        public int Id { get; set; }
        public string PositionName { get; set; }
        public Subdivision Subdivision { get; set; }
    }
}
