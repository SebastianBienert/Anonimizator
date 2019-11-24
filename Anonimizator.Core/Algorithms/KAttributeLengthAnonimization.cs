using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;

namespace Anonimizator.Core.Algorithms
{
    public class KAttributeLengthAnonimization<T> : IKAnonimization
    {
        public int ParameterK { get; }
        private readonly Expression<Func<Person, T>> _anonimizedExpression;
        private readonly Func<Person, T> _anonimizedProperty;

        public KAttributeLengthAnonimization(int parameterK, Expression<Func<Person, T>> anonimizedProperty)
        {
            ParameterK = parameterK;
            _anonimizedExpression = anonimizedProperty;
            _anonimizedProperty = anonimizedProperty.Compile();
        }

        public List<Person> GetAnonymizedData(IEnumerable<Person> people)
        {
            if (people == null || !people.Any())
                return new List<Person>();

            var groupsOrderedByLength = people.GroupBy(p =>
                {
                    var propertyInfo = Reflections.GetPropertyInfo(p, _anonimizedExpression);
                    return propertyInfo.GetValue(p).ToString().Length;
                })
                .Select(gPeople => new
                {
                    Value = gPeople.Select(_anonimizedProperty).First().ToString().Length,
                    People = gPeople.ToList(),
                    Count = gPeople.Count()
                })
                .OrderBy(p => p.Value);

            var result = new List<List<Person>>();
            var currentIntervalGroup = new List<Person>();

            foreach (var group in groupsOrderedByLength)
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
            if (currentIntervalGroup.Count < ParameterK && result.Any())
            {
                result.Last().AddRange(currentIntervalGroup);
            }
            else
            {
                result.Add(currentIntervalGroup);
            }
                

            var flattenedResult = result.Select(GetPeopleWithAnnonymazedAgeRange).SelectMany(x => x).ToList();
            return flattenedResult;
        }

        public List<Person> GetPeopleWithAnnonymazedAgeRange(List<Person> people)
        {
            var max = people.Select(_anonimizedProperty).Max(x => x.ToString().Length);
            var min = people.Select(_anonimizedProperty).Min(x => x.ToString().Length);

            var propertyInfo = Reflections.GetPropertyInfo(new Person(), _anonimizedExpression);

            var anonymzedPeople = people.Select(p =>
                {
                    var clone = p.Clone();
                    var anonymyzedValue = min == max ? min.ToString() : $"{min} - {max}";
                    propertyInfo.SetValue(clone, Convert.ChangeType($"{anonymyzedValue} letters", propertyInfo.PropertyType), null);
                    return clone;
                })
                .ToList();
            return anonymzedPeople;
        }
    }
}
