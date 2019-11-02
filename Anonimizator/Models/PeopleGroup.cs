using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonimizator.Models
{
    class PeopleGroup<T>
    {
        private readonly List<Person> _people;

        public T Value { get; }
        public IEnumerable<Person> People => _people;
        public int Count => _people.Count;

        public PeopleGroup(List<Person> people, T value)
        {
            _people = people;
            Value = value;
        }
    }
}
