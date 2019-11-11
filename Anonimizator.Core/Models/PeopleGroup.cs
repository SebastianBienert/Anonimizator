using System.Collections.Generic;

namespace Anonimizator.Core.Models
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
