using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestForSintezTech.Models;

namespace TestForSintezTech.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Position { get; set; }
        public int Subdivision { get; set; }

        public EmployeeViewModel() { }

        public EmployeeViewModel(Employee employee)
        {
            Id = employee.Id;
            FirstName = employee.FirstName;
            LastName = employee.LastName;
            Patronymic = employee.Patronymic;
            Age = employee.Age;
            Gender = employee.Gender;

            if (employee.Positions != null && employee.Positions.Any())
            {
                Position = employee.Positions[0].PositionName;
                if (employee.Positions[0].Subdivision != null)
                    Subdivision = employee.Positions[0].Subdivision.Id;
            }
        }

        public Employee ToEmployee()
        {
            return new Employee
            {
                Id = Id,
                FirstName = FirstName,
                LastName = LastName,
                Patronymic = Patronymic,
                Age = Age,
                Gender = Gender,
                Positions = new List<Position>()
            };
        }

        public Employee ToEmployee(Subdivision subdivision)
        {
            var employee = ToEmployee();

            employee.Positions.Add(new Position
            {
                Subdivision = subdivision,
                PositionName = Position
            });

            return employee;
        }
    }
}
