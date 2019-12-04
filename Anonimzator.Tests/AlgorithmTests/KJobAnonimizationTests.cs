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
    public class KJobAnonimizationTests
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

            new Person("K", "Malarz", "Olawa", "Julia", "Rehocik", "49"),
            new Person("K", "Malarz", "Olawa", "Julia", "Mothas", "49"),
            new Person("K", "Malarz", "Olawa", "Julia", "Albert", "49"),
            new Person("K", "Malarz", "Olawa", "Julia", "Nazerska", "49"),

            new Person("K", "Tancerz", "Olawa", "Julia", "Rehocik", "49"),
            new Person("K", "Muzyk", "Olawa", "Julia", "Mothas", "49"),
            new Person("K", "Muzyk", "Olawa", "Julia", "Albert", "49"),
            new Person("K", "Muzyk", "Olawa", "Julia", "Nazerska", "49"),
        };
        
        private readonly List<List<string>> _jobDictionary;

        public KJobAnonimizationTests()
        {
            var fileService = new FileService();
            _jobDictionary = fileService.GetDictionaryData(ConstantStrings.FILE_WITH_JOB_GENERALIZATION_DICTIONARY);
        }


        [TestMethod]
        public void GivenEmptyPeopleList_ShouldReturn_EmptyList()
        {
            //Arrange
            var algorithm = new KJobAnonimization(2, _jobDictionary);

            //Act
            var anonymzed = algorithm.GetAnonymizedData(new List<Person>());

            //Assert
            Assert.IsTrue(!anonymzed.Any());
        }

        [TestMethod]
        public void GivenKParameterOne_ShouldReturnTheSameList()
        {
            //Arrange
            var algorithm = new KJobAnonimization(1, _jobDictionary);

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
            var algorithm = new KJobAnonimization(parameterK, _jobDictionary);
            //Act
            var anonymzed = algorithm.GetAnonymizedData(_people);
            //Assert

            Assert.AreEqual(_people.Count, anonymzed.Count);
            //All Groups have at least K people
            Assert.IsTrue(anonymzed.GroupBy(x => x.Job).Select(g => g.Count()).All(c => c >= parameterK));
        }
    }
}
