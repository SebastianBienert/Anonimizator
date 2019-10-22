using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Models;

namespace Anonimizator.Algorithms
{
    public class KAgeAnonimzation_V2 : IKAnonimization
    {
        public int ParameterK { get; }
        public IEnumerable<Person> People { get; }

        public KAgeAnonimzation_V2(int parameterK, IEnumerable<Person> people)
        {
            ParameterK = parameterK;
            People = people;
        }

        public List<Person> GetAnonymizedData()
        {
            if(People == null || !People.Any())
                return new List<Person>();

            var groupsOrderdByAge = People.GroupBy(person => person.Age)
                                    .Select(gPeople => new
                                    {
                                        Age = gPeople.First().Age,
                                        People = gPeople.ToList(),
                                        Count = gPeople.Count()
                                    })
                                    .OrderBy(p => Convert.ToInt32(p.Age));

            var result = new List<List<Person>>();
            var currentIntervalGroup = new List<Person>();

            foreach (var group in groupsOrderdByAge)
            {
                if (currentIntervalGroup.Count < ParameterK)
                {
                    currentIntervalGroup.AddRange(group.People);
                }
                else
                {
                    result.Add(currentIntervalGroup);
                    currentIntervalGroup = new List<Person>(group.People);
                }
            }
            //Handle last left group
            if(currentIntervalGroup.Count < ParameterK)
                result.Last().AddRange(currentIntervalGroup);
            else
                result.Add(currentIntervalGroup);

            var flattenedResult = result.Select(GetPeopleWithAnnonymazedAgeRange).SelectMany(x => x).ToList();
            return flattenedResult;
        }

        public List<Person> GetPeopleWithAnnonymazedAgeRange(List<Person> people)
        {
            var max = people.Max(p => Convert.ToInt32(p.Age));
            var min = people.Min(p => Convert.ToInt32(p.Age));

            var anonymzedPeople = people.Select(p => new Person(p.Gender, p.Job, p.City, p.FirstName, p.Surname,
                                                                min == max ? min.ToString() : $"{min} - {max}"))
                                        .ToList();
            return anonymzedPeople;
        }

    }
}
