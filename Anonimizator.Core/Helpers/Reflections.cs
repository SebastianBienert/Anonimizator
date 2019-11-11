using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Anonimizator.Core.Models;

namespace Anonimizator.Core.Helpers
{
    public static class Reflections
    {
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            if (!(propertyLambda.Body is MemberExpression member))
                throw new ArgumentException(
                    $"Expression '{propertyLambda.ToString()}' refers to a method, not a property.");

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(
                    $"Expression '{propertyLambda.ToString()}' refers to a field, not a property.");

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(
                    $"Expression '{propertyLambda.ToString()}' refers to a property that is not from type {type}.");

            return propInfo;
        }

        public static string GetPersonProperties(this Person person, params Expression<Func<Person, object>>[] properites)
        {
            //TODO SOMETHING BETTER - return object with only specified properties
            var result = properites.Aggregate(new StringBuilder(),
                                            (sum, prop) => sum.Append(GetPropertyInfo(person, prop).GetValue(person).ToString()))
                                   .ToString();
            return result;
        }

    }
}
