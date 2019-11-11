using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Anonimizator.Algorithms;
using Anonimizator.Core.Models;
using Neleus.LambdaCompare;

namespace Anonimizator.Core
{
    public class AlgorithmsEnumeratorBuilder
    {
        private int _maxKParameter = 20;
        private readonly Dictionary<Expression<Func<Person, object>>, List<List<string>>> _dictionaries;
        private Expression<Func<Person, object>>[] _pidProperties;

        public AlgorithmsEnumeratorBuilder()
        {
            _dictionaries = new Dictionary<Expression<Func<Person, object>>, List<List<string>>>(new ExpressionEqualityComparer());
        }

        public AlgorithmsEnumeratorBuilder SetMaximumKParameter(int maxK)
        {
            _maxKParameter = maxK;
            return this;
        }

        public AlgorithmsEnumeratorBuilder AddDictionary(Expression<Func<Person, object>> property, List<List<string>> dictionary)
        {
            _dictionaries[property] = dictionary;
            return this;
        }

        public AlgorithmsEnumeratorBuilder SetPID(params Expression<Func<Person, object>>[] pid)
        {
            _pidProperties = pid;
            return this;
        }

        public AlgorithmsEnumerator Build()
        {
            var enumerator = new AlgorithmsEnumerator(_maxKParameter, _dictionaries, _pidProperties);
            return enumerator;
        }

    }

    public sealed class ExpressionEqualityComparer : IEqualityComparer<Expression<Func<Person, object>>>
    {
        public bool Equals(Expression<Func<Person, object>> x, Expression<Func<Person, object>> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return Lambda.Eq(x, y);
        }

        public int GetHashCode(Expression<Func<Person, object>> obj)
        {
            var memberNameHashCode = ((MemberExpression) obj.Body).Member.Name.GetHashCode();
            return memberNameHashCode;
        }
    }
}
