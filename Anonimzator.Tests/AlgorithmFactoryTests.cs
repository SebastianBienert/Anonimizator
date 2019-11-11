﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Anonimizator.Algorithms;
using Anonimizator.Models;
using Anonimizator.Services;
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
            var algoFactory = new KAnonimizationFactory(2, new FileService(), _pidProperties);
            foreach (var prop in algoFactory)
            {
                collection.Add(prop);
            }

            Assert.IsTrue(collection.Count == (1 << _pidProperties.Length) - 1);
        }

        [TestMethod]
        public void GivenPropeties_ShouldReturnAllCombinationsUsingWhileLoop()
        {
            var collection = new List<List<IKAnonimization>>();
            var algoFactory = new KAnonimizationFactory(2, new FileService(), _pidProperties);

            using (var algosIterator = algoFactory.GetEnumerator())
            {
                while (algosIterator.MoveNext())
                {
                    collection.Add(algosIterator.Current);
                }
            }
            Assert.IsTrue(collection.Count == (1 << _pidProperties.Length) - 1);
        }


    }
}
