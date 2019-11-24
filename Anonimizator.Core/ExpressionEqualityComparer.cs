using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Anonimizator.Core.Models;
using Neleus.LambdaCompare;

namespace Anonimizator.Core
{
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