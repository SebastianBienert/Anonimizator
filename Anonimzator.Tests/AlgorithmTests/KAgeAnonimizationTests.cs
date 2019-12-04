using System.Collections.Generic;
using System.Linq;
using Anonimizator.Core.Algorithms;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;
using Anonimizator.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anonimzator.Tests.AlgorithmTests
{
    [TestClass]
    public class KAgeAnonimizationTests
    {
        private List<Person> _people = new List<Person>()
        {
            new Person("M", "Inzynier", "Wroclaw", "Jan", "Nowak", "7"),
            new Person("M", "Inzynier", "Katowice", "Adam", "Nowakowski", "11"),
            new Person("M", "Inzynier", "Wroclaw", "Kamil", "Nowak", "15"),
            new Person("M", "Inzynier", "Czestochowa", "Jakub", "Kowal", "15"),

            new Person("M", "Programista", "Wroclaw", "Szymon", "Kowal", "19"),
            new Person("M", "Programista", "Wroclaw", "Filip", "Kowalski", "22"),
            new Person("M", "Architekt", "Rybnik", "Mikolaj", "Kowalski", "30"),
            new Person("K", "Architekt", "Olawa", "Julia", "Wojcik", "49"),

            new Person("K", "Malarz", "Olawa", "Julia", "Rehocik", "11"),
            new Person("K", "Malarz", "Olawa", "Julia", "Mothas", "15"),
            new Person("K", "Malarz", "Olawa", "Julia", "Albert", "13"),
            new Person("K", "Malarz", "Olawa", "Julia", "Nazerska", "17"),

            new Person("K", "Tancerz", "Olawa", "Julia", "Rehocik", "5"),
            new Person("K", "Muzyk", "Olawa", "Julia", "Mothas", "24"),
            new Person("K", "Muzyk", "Olawa", "Julia", "Albert", "45"),
            new Person("K", "Muzyk", "Olawa", "Julia", "Nazerska", "33"),

            new Person("K", "Malarz", "Olawa", "Julia", "Rehocik", "56"),
            new Person("K", "Malarz", "Olawa", "Julia", "Mothas", "78"),
            new Person("K", "Malarz", "Olawa", "Julia", "Albert", "98"),
            new Person("K", "Malarz", "Olawa", "Julia", "Nazerska", "78"),

            new Person("K", "Tancerz", "Olawa", "Julia", "Rehocik", "45"),
            new Person("K", "Muzyk", "Olawa", "Julia", "Mothas", "34"),
            new Person("K", "Muzyk", "Olawa", "Julia", "Albert", "23"),
            new Person("K", "Muzyk", "Olawa", "Julia", "Nazerska", "22"),
        };

        [TestMethod]
        public void GivenEmptyPeopleList_ShouldReturn_EmptyList()
        {
            //Arrange
            var algorithm = new KAgeAnonimization(2);

            //Act
            var anonymzed = algorithm.GetAnonymizedData(new List<Person>());

            //Assert
            Assert.IsTrue(!anonymzed.Any());
        }

        [TestMethod]
        public void GivenKParameterOne_ShouldReturnTheSameList()
        {
            //Arrange
            var algorithm = new KAgeAnonimization(1);

            //Act
            var anonymzed = algorithm.GetAnonymizedData(new List<Person>());

            //Assert
            Assert.IsTrue(anonymzed.All(p => p.Age == _people.First(x => x.FirstName == p.FirstName && x.Surname == p.Surname).Age));
        }

        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        [DataTestMethod]
        public void GivenKParameter_GreaterThan_1_ShouldReturnAnonymyzedList(int parameterK)
        {
            //Arrange
            var algorithm = new KAgeAnonimization(parameterK);
            //Act
            var anonymzed = algorithm.GetAnonymizedData(_people);
            //Assert

            Assert.AreEqual(_people.Count, anonymzed.Count);
            //All Groups have at least K people
            Assert.IsTrue(anonymzed.GroupBy(x => x.Age).Select(g => g.Count()).All(c => c >= parameterK));
        }
    }
}
