using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Models;

namespace Anonimizator.Algorithms
{
    public class CharacterMasking
    {
        public IEnumerable<Person> People { get; }

        public CharacterMasking(IEnumerable<Person> people)
        {
            People = people;
        }

        public List<Person> GetAnonymizedData(string columnName)
        {
            return People.Select(p =>
                {
                    p.GetType().GetProperty(columnName).SetValue(p, "*");
                    return p;
                })
                .ToList();
        }
    }
}
