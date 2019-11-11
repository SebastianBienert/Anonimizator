using System;

namespace Anonimizator.Core.Models
{
    public class Person : ICloneable
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
            return new Person(this.Gender, this.Job, this.City, this.FirstName, this.Surname, this.Age);
        }

        object ICloneable.Clone()
        {
            return new Person(this.Gender, this.Job, this.City, this.FirstName, this.Surname, this.Age);
        }
    }
}
