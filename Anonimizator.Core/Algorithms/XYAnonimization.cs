using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Anonimizator.Algorithms;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;

namespace Anonimizator.Core.Algorithms
{
    public class XYAnonimization : IKAnonimization
    {
        public int ParameterK { get; }
        public List<Func<Person, object>> _xProperties { get; }
        public List<Expression<Func<Person, object>>> _xExpressions { get;  }
        public List<Func<Person, object>> _yProperties { get; }
        public List<Expression<Func<Person, object>>> _yExpressions { get; }
        private readonly AlgorithmsEnumerator _algorithmsEnumerator;

        public XYAnonimization(int parameterK, List<List<string>> jobDictionary,
            List<List<string>> cityDictionary, IEnumerable<Expression<Func<Person, object>>> xProperties,
            IEnumerable<Expression<Func<Person, object>>> yProperties)
        {
            ParameterK = parameterK;
            _xExpressions = xProperties.ToList();
            _xProperties = xProperties.Select(x => x.Compile()).ToList();
            _yExpressions = yProperties.ToList();
            _yProperties = yProperties.Select(x => x.Compile()).ToList();

            if (_yExpressions.Intersect(_xExpressions, new ExpressionEqualityComparer()).Any())
            {
                throw new Exception("X and Y sets have to be disjoint");
            }

            _algorithmsEnumerator = new AlgorithmsEnumeratorBuilder()
                .SetMaximumKParameter(100)
                .SetPID(_xExpressions.ToArray())
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
            return groups.All(g =>
            {
                var yPropertiesGroups = g.People.GroupBy(p => p.GetPersonProperties(_yExpressions.ToArray())).ToList();
                return yPropertiesGroups.Count >= ParameterK;
            });
        }

        private List<PeopleGroup<string>> GetGroupedPeople(IEnumerable<Person> people)
        {
            var groups = people.GroupBy(p => p.GetPersonProperties(_xExpressions.ToArray()))
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
