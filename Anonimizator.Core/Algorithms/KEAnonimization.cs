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
    public class KEAnonimization : IKAnonimization
    {
        public int ParameterK { get; }
        public int ParameterE { get; }
        public List<Expression<Func<Person, object>>> _xExpressions { get; }
        public List<Func<Person, object>> _xProperties { get; }
        public Expression<Func<Person, object>> _anonymyzedExpression { get; }
        public Func<Person, object> _anonymyzedProperty { get; }
        private readonly AlgorithmsEnumerator _algorithmsEnumerator;


        public KEAnonimization(int parameterK, int parameterE, 
            List<List<string>> jobDictionary, List<List<string>> cityDictionary,
            IEnumerable<Expression<Func<Person, object>>> pid,
            Expression<Func<Person, object>> anonymyzedProperty)

        {
            ParameterK = parameterK;
            ParameterE = parameterE;
            _xExpressions = pid.ToList();
            _xProperties = pid.Select(p => p.Compile()).ToList();
            _anonymyzedExpression = anonymyzedProperty;
            _anonymyzedProperty = anonymyzedProperty.Compile();

            _algorithmsEnumerator = new AlgorithmsEnumeratorBuilder()
                .SetMaximumKParameter(100)
                .SetPID(pid.ToArray())
                .AddDictionary(p => p.City, cityDictionary)
                .AddDictionary(p => p.Job, jobDictionary)
                .Build();
        }

        public List<Person> GetAnonymizedData(IEnumerable<Person> people)
        {
            if (people == null || !people.Any())
                return new List<Person>();

            if (people.Select(_anonymyzedProperty).Any(p => !int.TryParse(p.ToString(), out _)))
                throw new Exception("Anonymyzed property has to be integer");
        

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
                var min = g.People.Select(_anonymyzedProperty).Min(p => Convert.ToInt32(p.ToString()));
                var max = g.People.Select(_anonymyzedProperty).Max(p => Convert.ToInt32(p.ToString()));
                return max - min >= ParameterE;
            });

            var kParameterCondition = groups.All(g =>
            {
                var uniqueValues = g.People.GroupBy(_anonymyzedProperty).Count();
                return uniqueValues >= ParameterK;
            });

            return eParameterCondition && kParameterCondition;
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
