using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Anonimizator.Core.Algorithms;
using Anonimizator.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anonimzator.Tests.AlgorithmTests
{
    [TestClass]
    public class KNumberAnonimizationTests
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
            var ageAnonimizator = new KNumberAnonimization<string>(2, p => p.Age);
            var anonymzed = ageAnonimizator.GetAnonymizedData(new List<Person>());
            Assert.IsTrue(!anonymzed.Any());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GivenInvalidData_ShouldThrowException()
        {
            var invalidData =_people.Union(new List<Person> {new Person("K", "Malarz", "Olawa", "Julia", "Wojcik", "invalid")});
            var ageAnonimizator = new KNumberAnonimization<string>(1, p => p.Age);
            var anonymzed = ageAnonimizator.GetAnonymizedData(invalidData);
        }

        [TestMethod]
        public void GivenKParameterOne_ShouldReturnTheSameList()
        {
            var ageAnonimizator = new KNumberAnonimization<string>(1, p => p.Age);
            var anonymzed = ageAnonimizator.GetAnonymizedData(_people);
            Assert.IsTrue(anonymzed.All(p => p.Age == _people.First(x => x.FirstName == p.FirstName && x.Surname == p.Surname).Age));
        }

        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataTestMethod]
        public void GivenKParameter_GreaterThan_1_ShouldReturnAnonymyzedList(int parameterK)
        {
            var ageAnonimizator = new KNumberAnonimization<string>(parameterK, p => p.Age);
            var anonymzed = ageAnonimizator.GetAnonymizedData(_people);
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
            var ageAnonimizator = new KAgeAnonimization(parameterK);
            var anonymzed = ageAnonimizator.GetAnonymizedData(_people);
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
