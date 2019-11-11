using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;

namespace Anonimizator.Core.Algorithms
{
    public class KNumberAnonimization<T> : IKAnonimization
    {
        public int ParameterK { get; }
        private readonly Expression<Func<Person, T>> _anonimizedExpression;
        private readonly Func<Person, T> _anonimizedProperty;

        public KNumberAnonimization(int parameterK, Expression<Func<Person, T>> anonimizedExpression)
        {
            ParameterK = parameterK;
            _anonimizedExpression = anonimizedExpression;
            _anonimizedProperty = anonimizedExpression.Compile();
        }

        public List<Person> GetAnonymizedData(IEnumerable<Person> people)
        {
            var dataValid = ValidateProperty(people.ToList());
            if(!dataValid)
                throw new Exception($"Wrong format of data for ${_anonimizedExpression.Name} property in {this.GetType()} class");

            if (people == null || !people.Any())
                return new List<Person>();

            var groups = people.GroupBy(_anonimizedProperty)
                                    .Select(gPeople => new
                                    {
                                        Value = gPeople.Select(_anonimizedProperty).First(),
                                        People = gPeople.ToList(),
                                        Count = gPeople.Count()
                                    })
                                    .OrderBy(p => Convert.ToInt32(p.Value));

            var result = new List<List<Person>>();
            var currentIntervalGroup = new List<Person>();

            foreach (var group in groups)
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

            var flattenedResult = result.Select(GetPeopleWithAnonymizedProperty).SelectMany(x => x).ToList();
            return flattenedResult;
        }

        public List<Person> GetPeopleWithAnonymizedProperty(List<Person> people)
        {
            var max = people.Select(_anonimizedProperty).Max(v => Convert.ToInt32(v));
            var min = people.Select(_anonimizedProperty).Min(v => Convert.ToInt32(v));
            var propertyInfo = Reflections.GetPropertyInfo(new Person(), _anonimizedExpression);
            var anonymizedPeople = people.Select(p =>
                {
                    var clone = p.Clone();
                    var anonymizedValue = min == max ? min.ToString() : $"{min} - {max}";
                    propertyInfo.SetValue(clone, Convert.ChangeType(anonymizedValue, propertyInfo.PropertyType), null);
                    return clone;
                })
                .ToList();
            return anonymizedPeople;
        }

        private bool ValidateProperty(List<Person> people)
        {
            var validationResult = people.Select(_anonimizedProperty).All(x => Int32.TryParse(x.ToString(), out _));
            return validationResult;
        }

    }
}
