using System.Collections.Generic;
using System.Linq;
using Anonimizator.Core.Algorithms;
using Anonimizator.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anonimzator.Tests.AlgorithmTests
{
    /// <summary>
    /// Summary description for CharacterMaskingTests
    /// </summary>
    [TestClass]
    public class CharacterMaskingTests
    {
        private readonly List<Person> _people = new List<Person>()
        {
            new Person("M", "Inzynier", "Wroclaw", "Jan", "Nowak", "7"),
            new Person("M", "Inzynier", "Katowice", "Adam", "Kowalski", "11"),
            new Person("M", "Inzynier", "Wroclaw", "Kamil", "Malinowski", "15"),
            new Person("M", "Inzynier", "Czestochowa", "Jakub", "Jagiel", "15"),

            new Person("M", "Programista", "Wroclaw", "Szymon", "Osowski", "19"),
            new Person("M", "Programista", "Wroclaw", "Filip", "Nowak", "22"),
            new Person("M", "Malarz", "Rybnik", "Mikolaj", "Kowalski", "30"),
            new Person("K", "Malarz", "Olawa", "Julia", "Wojcik", "49"),
        };


        [TestMethod]
        public void GivenEmptyPeopleList_ShouldReturn_EmptyList()
        {
            var algorithm = new CharacterMasking<string>(p => p.Age);
            var anonymized = algorithm.GetAnonymizedData(new List<Person>());
            Assert.IsTrue(!anonymized.Any());
        }

        [TestMethod]
        public void GivenNonEmptyList_ShouldReturnMaskedList()
        {
            var algorithm = new CharacterMasking<string>(p => p.Age);
            var anonymized = algorithm.GetAnonymizedData(new List<Person>());
            Assert.IsTrue(anonymized.All(p => p.Age == "*"));
        }

    }
}
