using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Anonimizator.Algorithms;
using Anonimizator.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anonimzator.Tests
{
    [TestClass]
    public class KAgeAnonimizationTests
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
            var ageAnonimizator = new KAgeAnonimzation_V2(2, new List<Person>());

            //Act
            var anonymzed = ageAnonimizator.GetAnonymizedData();

            //Assert
            Assert.IsTrue(!anonymzed.Any());
        }

        [TestMethod]
        public void GivenKParameterOne_ShouldReturnTheSameList()
        {
            //Arrange
            var ageAnonimizator = new KAgeAnonimzation_V2(1, _people);

            //Act
            var anonymzed = ageAnonimizator.GetAnonymizedData();

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
            var ageAnonimizator = new KAgeAnonimzation_V2(parameterK, _people);
            //Act
            var anonymzed = ageAnonimizator.GetAnonymizedData();
            //Assert

            Assert.AreEqual(_people.Count, anonymzed.Count);
            //All Groups have at least K people
            Assert.IsTrue(anonymzed.GroupBy(x => x.Age).Select(g => g.Count()).All(c => c >= parameterK));   
            //No overlapping groups
            Assert.IsTrue(!AreIntervalsOverlapping(anonymzed));
        }

        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataTestMethod]
        public void MATI_GivenKParameter_GreaterThan_1_ShouldReturnAnonymyzedList(int parameterK)
        {
            //Arrange
            var ageAnonimizator = new KAgeAnonimization(parameterK, _people);
            //Act
            var anonymzed = ageAnonimizator.GetAnonymizedData();
            //Assert

            Assert.AreEqual(_people.Count, anonymzed.Count);
            //All Groups have at least K people
            Assert.IsTrue(anonymzed.GroupBy(x => x.Age).Select(g => g.Count()).All(c => c >= parameterK));
            //No overlapping groups
            Assert.IsTrue(!AreIntervalsOverlapping(anonymzed));
        }

        private Boolean AreIntervalsOverlapping(List<Person> people)
        {
            var groups = people.Select(p => p.Age).Distinct();
            var numbersUsed = groups.SelectMany(g => Regex.Matches(g, @"\d+").OfType<Match>()
                                                                         .Select(m => Convert.ToInt32(m.Value)))
                                    .ToList();

            return numbersUsed.Count != numbersUsed.Distinct().Count();

        }


    }
}
