using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Anonimizator.Algorithms;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;

namespace Anonimizator.Core.Algorithms
{
    public class AKAnonimization : IKAnonimization
    {
        public int ParameterK { get; }
        public double ParameterAlpha { get; }
        public string AttributeValue { get; }
        public List<Func<Person, object>> _properties { get; }
        public List<Expression<Func<Person, object>>> _expressions { get; }
        public Expression<Func<Person, object>> _selectedAttributeExpression { get; }
        public Func<Person, object> _selectedAttributeProperty { get; }
        private readonly AlgorithmsEnumerator _algorithmsEnumerator;

        public AKAnonimization(int parameterK, double parameterAlpha, string attributeValue, List<List<string>> jobDictionary,
            List<List<string>> cityDictionary, Expression<Func<Person, object>> selectedAttribute,
            params Expression<Func<Person, object>>[] pidProperties)
        {
            ParameterK = parameterK;
            ParameterAlpha = parameterAlpha;
            AttributeValue = attributeValue;
            _expressions = pidProperties.ToList();
            _properties = pidProperties.Select(x => x.Compile()).ToList();
            _selectedAttributeExpression = selectedAttribute;
            _selectedAttributeProperty = selectedAttribute.Compile();
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
            var eParameterCondition = groups.All(g =>
            {
                var numberItems = g.People.Select(_selectedAttributeProperty).Count(p => p.ToString() == AttributeValue);
                var groupSize = g.People.Select(_selectedAttributeProperty).Count();
                return numberItems / (double)groupSize <= ParameterAlpha;
            });

            var kParameterCondition = groups.All(g => g.Count >= ParameterK);

            return eParameterCondition && kParameterCondition;
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
