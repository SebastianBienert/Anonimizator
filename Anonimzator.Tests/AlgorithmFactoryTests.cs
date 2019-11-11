using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Anonimizator.Algorithms;
using Anonimizator.Core.Algorithms;
using Anonimizator.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anonimzator.Tests
{
    [TestClass]
    public class AlgorithmFactoryTests
    {
        private Expression<Func<Person, object>>[] _pidProperties = {
            p => p.FirstName,
            p => p.Surname,
            p => p.Age
        };

        [TestMethod]
        public void GivenPropeties_ShouldReturnAllCombinations()
        {
            var collection = new List<List<IKAnonimization>>();
            var algoFactory = new AlgorithmsEnumerator(2, null, _pidProperties);
            foreach (var prop in algoFactory)
            {
                collection.Add(prop);
            }

            Assert.IsTrue(collection.Count == (1 << _pidProperties.Length));
        }

        [TestMethod]
        public void GivenPropeties_ShouldReturnAllCombinationsUsingWhileLoop()
        {
            var collection = new List<List<IKAnonimization>>();
            var algoFactory = new AlgorithmsEnumerator(2, null, _pidProperties);

            using (var algosIterator = algoFactory.GetEnumerator())
            {
                while (algosIterator.MoveNext())
                {
                    collection.Add(algosIterator.Current);
                }
            }
            Assert.IsTrue(collection.Count == (1 << _pidProperties.Length));
        }


    }
}
