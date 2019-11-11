using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;

namespace Anonimizator.Core.Algorithms
{
    public class CharacterMasking<T> : IKAnonimization
    {
        private readonly Expression<Func<Person, T>> _anonimizedExpression;
        private readonly string _propertyName;

        public CharacterMasking(Expression<Func<Person, T>> anonimizedProperty)
        {
            _anonimizedExpression = anonimizedProperty;
        }

        public CharacterMasking(string propertyName)
        {
            _propertyName = propertyName;
        }

        public List<Person> GetAnonymizedData(IEnumerable<Person> people)
        {
            List<Person> anonymzedData;
            if (_propertyName == null)
            {
                var propertyInfo = Reflections.GetPropertyInfo(new Person(), _anonimizedExpression);
                anonymzedData = people.Select(p =>
                    {
                        var clone = p.Clone();
                        propertyInfo.SetValue(clone, Convert.ChangeType("*", propertyInfo.PropertyType), null);
                        return clone;
                    })
                    .ToList();
            }
            else
            {
                anonymzedData = people.Select(p =>
                    {
                        var clone = p.Clone();
                        clone.GetType().GetProperty(_propertyName)?.SetValue(clone, "*");
                        return clone;
                    })
                    .ToList();
            }
            
            return anonymzedData;
        }
    }
}
