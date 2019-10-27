using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonimizator.Models
{
    public class Person
    {
        public string Gender { get; set; }

        public string Job { get; set; }

        public string City { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Age { get; set; }

        public Person(string gender, string job, string city, string firstName, string surname, string age)
        {
            Gender = gender;
            Job = job;
            City = city;
            FirstName = firstName;
            Surname = surname;
            Age = age;
        }

        public Person()
        {

        }

        public Person Clone()
        {
            return new Person(this.Age, this.Job, this.City, this.FirstName, this.Surname, this.Age);
        }


    }
}
