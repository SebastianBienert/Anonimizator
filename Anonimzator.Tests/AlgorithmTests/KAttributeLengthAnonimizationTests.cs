using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Anonimizator.Core.Algorithms;
using Anonimizator.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anonimzator.Tests.AlgorithmTests
{
    /// <summary>
    /// Summary description for asd
    /// </summary>
    [TestClass]
    public class KAttributeLengthAnonimizationTests
    {
        private List<Person> _people = new List<Person>()
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
            //Arrange
            var algorithm = new KAttributeLengthAnonimization<string>(2, p => p.FirstName);

            //Act
            var anonymzed = algorithm.GetAnonymizedData(new List<Person>());

            //Assert
            Assert.IsTrue(!anonymzed.Any());
        }

        [TestMethod]
        public void GivenKParameterOne_ShouldReturnTheSameList()
        {
            //Arrange
            var algorithm = new KAttributeLengthAnonimization<string>(2, p => p.FirstName);
            //Act
            var anonymzed = algorithm.GetAnonymizedData(new List<Person>());

            //Assert
            Assert.IsTrue(anonymzed.All(p => p.Age == _people.First(x => x.FirstName == p.FirstName && x.Surname == p.Surname).Age));
        }

        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataTestMethod]
        public void GivenKParameter_GreaterThan_1_ShouldReturnAnonymyzedList(int parameterK)
        {
            //Arrange
            var algorithm = new KAttributeLengthAnonimization<string>(parameterK, p => p.FirstName);
            //Act
            var anonymzed = algorithm.GetAnonymizedData(_people);
            //Assert

            Assert.AreEqual(_people.Count, anonymzed.Count);
            //All Groups have at least K people
            Assert.IsTrue(anonymzed.GroupBy(x => x.FirstName).Select(g => g.Count()).All(c => c >= parameterK));
            //No overlapping groups
            Assert.IsTrue(!AreIntervalsOverlapping(anonymzed, p => p.FirstName));

        }

        private Boolean AreIntervalsOverlapping(List<Person> people, Func<Person, string> property)
        {
            var groups = people.Select(property).Distinct();
            var numbersUsed = groups.SelectMany(g => Regex.Matches(g, @"\d+")
                                                          .OfType<Match>()
                                                          .Select(m => Convert.ToInt32(m.Value)))
                                    .ToList();

            return numbersUsed.Count != numbersUsed.Distinct().Count();

        }
    }
}
