using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Helpers;
using Anonimizator.Models;

namespace Anonimizator.Algorithms
{
    public class CharacterMasking<T> : IKAnonimization
    {
        private readonly Expression<Func<Person, T>> _anonimizedExpression;

        public CharacterMasking(Expression<Func<Person, T>> anonimizedProperty)
        {
            _anonimizedExpression = anonimizedProperty;
        }

        public List<Person> GetAnonymizedData(IEnumerable<Person> people)
        {
            var propertyInfo = Reflections.GetPropertyInfo(new Person(), _anonimizedExpression);
            var anonymzedData = people.Select(p =>
                {
                    var clone = p.Clone();
                    propertyInfo.SetValue(clone, Convert.ChangeType("*", propertyInfo.PropertyType), null);
                    return clone;
                })
                .ToList();
            return anonymzedData;
        }
    }
}
