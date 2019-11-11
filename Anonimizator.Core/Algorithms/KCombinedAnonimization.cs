using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Anonimizator.Algorithms;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;

namespace Anonimizator.Core.Algorithms
{
    public class KCombinedAnonimization : IKAnonimization
    {
        public int ParameterK { get; }
        public List<Func<Person, object>> _properties { get; set; }
        public List<Expression<Func<Person, object>>> _expressions { get; set; }
        private readonly AlgorithmsEnumerator _algorithmsEnumerator;

        public KCombinedAnonimization(int parameterK, List<List<string>> jobDictionary,
            List<List<string>> cityDictionary, params Expression<Func<Person, object>>[] pidProperties)
        {
            ParameterK = parameterK;
            _expressions = pidProperties.ToList();
            _properties = pidProperties.Select(x => x.Compile()).ToList();
            _algorithmsEnumerator = new AlgorithmsEnumeratorBuilder()
                .SetMaximumKParameter(100)
                .SetPID(pidProperties)
                .AddDictionary(p => p.City, cityDictionary)
                .AddDictionary(p => p.Job, jobDictionary)
                .Build();
        }

        public List<Person> GetAnonymizedData(IEnumerable<Person> people)
        {
            if (people == null || !people.Any())
                return new List<Person>();

            var groups = GetGroupedPeople(people);

            foreach (var algorithms in _algorithmsEnumerator)
            {
                var anonymzedData = algorithms.Aggregate(people.Clone(), (acc, algo) => algo.GetAnonymizedData(acc));
                groups = GetGroupedPeople(anonymzedData);
                if (IsListAnonymized(groups))
                    break;
            }
  
            return groups.SelectMany(x => x.People).ToList();
        }

        private bool IsListAnonymized(IEnumerable<PeopleGroup<string>> groups)
        {
            return groups.All(g => g.Count >= ParameterK);
        }

        private List<PeopleGroup<string>> GetGroupedPeople(IEnumerable<Person> people)
        {
            var groups = people.GroupBy(p => p.GetPersonProperties(_expressions.ToArray()))
                .Select(gPeople =>
                {
                    var group = new PeopleGroup<string>
                    (
                        people: gPeople.ToList(),
                        value: gPeople.Key
                    );
                    return group;
                })
                .OrderBy(p => p.Value)
                .ToList();

            return groups;
        }




    }
}
