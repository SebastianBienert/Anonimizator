using System;
using System.Linq.Expressions;
using Anonimizator.Core;
using Anonimizator.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anonimzator.Tests
{
    /// <summary>
    /// Summary description for ExpressionDictionaryTests
    /// </summary>
    [TestClass]
    public class ExpressionDictionaryTests
    {

        [TestMethod]
        public void GivenSameExpressions_ComparerShouldReturnEqualTrue()
        {
            var comparer = new ExpressionEqualityComparer(); 
            Expression<Func<Person, object>> x = p => p.Age;
            Expression<Func<Person, object>> y = p => p.Age;

            var areEqual = comparer.Equals(x, y);

            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void GivenDifferentExpressions_ComparerShouldReturnEqualFalse()
        {
            var comparer = new ExpressionEqualityComparer();
            Expression<Func<Person, object>> x = p => p.Surname;
            Expression<Func<Person, object>> y = p => p.Age;

            var areEqual = comparer.Equals(x, y);

            Assert.IsFalse(areEqual);
        }

        [TestMethod]
        public void GivenSameExpressions_ComparerShouldReturnSameHashCode()
        {
            var comparer = new ExpressionEqualityComparer();
            Expression<Func<Person, object>> x = p => p.Age;
            Expression<Func<Person, object>> y = p => p.Age;

            var xHashCode = comparer.GetHashCode(x);
            var yHashCode = comparer.GetHashCode(y);

            Assert.IsTrue(xHashCode == yHashCode);
        }

        [TestMethod]
        public void GivenDifferentExpressions_ComparerShouldReturnDifferentHashCode()
        {
            var comparer = new ExpressionEqualityComparer();
            Expression<Func<Person, object>> x = p => p.Age;
            Expression<Func<Person, object>> y = p => p.Surname;

            var xHashCode = comparer.GetHashCode(x);
            var yHashCode = comparer.GetHashCode(y);

            Assert.IsFalse(xHashCode == yHashCode);
        }

    }
}
